import { useEffect, useRef, useState } from "react";
import OlMap from "ol/Map";
import Feature from "ol/Feature";
import VectorSource from "ol/source/Vector";
import VectorLayer from "ol/layer/Vector";
import { fromLonLat } from "ol/proj";

import { VehicleInfoPopup } from "./VehicleInfoPopup";
import { buildVehicleStyle } from "../../../../utils/mapStyle";
import Point from "ol/geom/Point";

import { useTransitStopLoader, transitStopToFeature } from "../hooks/useTransitStopLoader";
import { useTransitStopDraw } from "../hooks/useTransitStopDraw";
import { useTransitStopClick } from "../hooks/useTransitStopClick";
import { useTransitRoutes } from "../hooks/useTransitRoutes";
import { useTransitRouteLines } from "../hooks/useTransitRouteLines";
import { useRouteSimulation } from "../hooks/useRouteSimulation";
import { useVehicleClick } from "../hooks/useVehicleClick";
import { useTransitRouteClick } from "../hooks/useTransitRouteClick";


import { StopFormModal } from "./StopFormModal";
import { StopInfoPopup } from "./StopInfoPopup";
import { RouteManagementPanel } from "./RouteManagementPanel";
import type { PendingStop } from "../types";
import type { UserLookupEntry } from "../../core/hooks/useUserLookup";
import { createRouteSimulationConnection } from "../../../../shared/api/signalR";
import { RouteInfoPopup } from "./RouteInfoPopup";
import { createTransitStop, getRouteSimulationStatus } from "../../../../shared/api/transitService";
import { VehicleSelectionModal } from "./VehicleSelectionModal";


type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;
interface ToastState { message: string; type: "success" | "error"; }

interface TransitModuleProps {
  map:                    OlMap | null;
  apiFetch:               ApiFetch;
  userLookup:             Map<number, UserLookupEntry>;
  roles:                  string[];
  userId:                 number | null;
  isPlainUser:            boolean;
  otherFlowsIdle:         boolean;
  drawActive:             boolean;
  onDrawActiveChange:     (active: boolean) => void;
  onFormOpenChange:       (open: boolean) => void;
  onToast:                (toast: ToastState) => void;
  routeManagementOpen:    boolean;
  onRouteManagementClose: () => void;
}

export function TransitModule({
  map, apiFetch, userLookup, roles, userId, isPlainUser,
  otherFlowsIdle, drawActive, onDrawActiveChange, onFormOpenChange, onToast,
  routeManagementOpen, onRouteManagementClose,
}: TransitModuleProps) {
  const [pendingStop,         setPendingStop]         = useState<PendingStop | null>(null);
  const [isSavingStop,        setIsSavingStop]        = useState(false);
  const [stopFormError,       setStopFormError]       = useState<string | null>(null);
  const [stopSelectedFeature, setStopSelectedFeature] = useState<Feature | null>(null);
  const [lockedRouteId,       setLockedRouteId]       = useState<number | null>(null);

  const stopSourceRef = useRef(new VectorSource());
  const stopLayerRef  = useRef<VectorLayer<VectorSource> | null>(null);
  const routeLineSourceRef = useRef(new VectorSource());
  const [routeLineLayer, setRouteLineLayer] = useState<VectorLayer<VectorSource> | null>(null);

  const vehicleSourceRef = useRef(new VectorSource());
  const [vehicleLayer, setVehicleLayer] = useState<VectorLayer<VectorSource> | null>(null);
  const connectionRef = useRef(createRouteSimulationConnection());

  const flowActive = drawActive || pendingStop !== null;

  useEffect(() => { onFormOpenChange(pendingStop !== null); }, [pendingStop]);

  useEffect(() => { if (!flowActive) setLockedRouteId(null); }, [flowActive]);

  // SignalR bağlantısının component unmount olduğunda düzgün kapatılması
  useEffect(() => {
    const connection = connectionRef.current;
    return () => { connection.stop(); };
  }, []);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: stopSourceRef.current, zIndex: 4 });
    map.addLayer(layer);
    stopLayerRef.current = layer;

    return () => {
      map.removeLayer(layer);
      stopLayerRef.current = null;
    };
  }, [map]);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: routeLineSourceRef.current, zIndex: 3 });
    map.addLayer(layer);
    setRouteLineLayer(layer);

    return () => {
      map.removeLayer(layer);
      setRouteLineLayer(null);
    };
  }, [map]);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: vehicleSourceRef.current, zIndex: 5 });
    map.addLayer(layer);
    setVehicleLayer(layer);

    return () => {
      map.removeLayer(layer);
      setVehicleLayer(null);
    };
  }, [map]);

  useTransitStopLoader(map, stopSourceRef.current, apiFetch);
  const { routes: transitRoutes, reload: reloadTransitRoutes } = useTransitRoutes(apiFetch);
  useTransitRouteLines(routeLineSourceRef.current, transitRoutes);

  const {
    trackedRouteId, vehiclePositions, startTracking, stopTracking,
    startSimulation, stopSimulation, fetchRunningVehicleIds,
  } = useRouteSimulation(connectionRef.current, apiFetch);

  const { selected: vehicleSelected, clear: clearVehicleSelected } = useVehicleClick({
    map, vehicleLayer, enabled: otherFlowsIdle && !flowActive,
  });

  const { selected: routeSelected, clear: clearRouteSelected } = useTransitRouteClick({
    map, routeLineLayer, enabled: otherFlowsIdle && !flowActive,
  });

  const canManageTransitRoutes =
    roles.includes("Admin") || roles.includes("Takım Lideri") || roles.includes("Ulaşım Operatörü");

  const [routeActionBusy, setRouteActionBusy] = useState(false);
  const [vehicleModal, setVehicleModal] = useState<{ routeId: number; mode: "start" | "stop" } | null>(null);
  const [modalRunningIds, setModalRunningIds] = useState<Set<number>>(new Set());

  async function handleStopSimulation(routeId: number) {
    const runningIds = await fetchRunningVehicleIds(routeId);
    setModalRunningIds(runningIds);
    setVehicleModal({ routeId, mode: "stop" });
  }

  function handleStartSimulation(routeId: number) {
    setModalRunningIds(new Set());
    setVehicleModal({ routeId, mode: "start" });
  }

  async function handleVehicleModalConfirm(vehicleIds: number[]) {
    if (!vehicleModal) return;
    const { routeId, mode } = vehicleModal;

    setRouteActionBusy(true);
    try {
      const results = mode === "start"
        ? await startSimulation(routeId, vehicleIds)
        : await stopSimulation(routeId, vehicleIds);

      const failed = results.filter((r) => !r.success);
      if (failed.length > 0) {
        onToast({
          message: `${failed.length} araç için işlem başarısız: ${failed.map((f) => f.error).join(", ")}`,
          type: "error",
        });
      }

      const succeeded = results.some((r) => r.success);
      if (succeeded) {
        const runningIds = await fetchRunningVehicleIds(routeId);
        setSelectedRouteRunning(runningIds.size > 0);
        if (runningIds.size > 0) await startTracking(routeId);
        else if (trackedRouteId === routeId) stopTracking();
      }

      setVehicleModal(null);
    } catch {
      onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setRouteActionBusy(false);
    }
  }

  const [selectedRouteRunning, setSelectedRouteRunning] = useState(false);

  useEffect(() => {
    if (!routeSelected) return;
    let cancelled = false;

    (async () => {
      try {
        const res = await getRouteSimulationStatus(apiFetch, routeSelected.routeId);
        if (!res.ok) { if (!cancelled) setSelectedRouteRunning(false); return; }

        const data = await res.json();
        if (!cancelled) setSelectedRouteRunning(Array.isArray(data) && data.length > 0);
      } catch {
        if (!cancelled) setSelectedRouteRunning(false);
      }
    })();

    return () => { cancelled = true; };
  }, [routeSelected]);

  // Canlı araç pozisyonu değiştikçe haritadaki feature'ı günceller
  useEffect(() => {
    const source = vehicleSourceRef.current;
    source.clear();

    for (const position of vehiclePositions.values()) {
      if (position.completed) continue;

      const feature = new Feature({
        geometry: new Point(fromLonLat([position.longitude, position.latitude])),
      });
      feature.set("routeId", position.routeId);
      feature.set("vehicleId", position.vehicleId);
      feature.set("plateNumber", position.plateNumber);
      feature.set("progressPercentage", position.progressPercentage);
      feature.setStyle(buildVehicleStyle());

      source.addFeature(feature);
    }
  }, [vehiclePositions]);

  useTransitStopDraw({
    map,
    source: stopSourceRef.current,
    active: drawActive,
    onDrawEnd: async (wkt, feature) => {
      onDrawActiveChange(false);

      try {
        const res = await apiFetch("/api/geo-permission/validate", {
          method: "POST",
          body:   JSON.stringify({ wktGeometry: wkt }),
        });

        if (!res.ok) {
          stopSourceRef.current.removeFeature(feature);
          const message = await res.text();
          onToast({ message: `Hata: ${message}`, type: "error" });
          return;
        }

        setPendingStop({ wkt, feature });
      } catch {
        stopSourceRef.current.removeFeature(feature);
        onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
      }
    },
  });

  const { selected: stopClickSelected, clear: clearStopSelected } = useTransitStopClick({
    map,
    stopLayer: stopLayerRef.current,
    enabled:   otherFlowsIdle && !flowActive && !isPlainUser,
  });

  useEffect(() => {
    setStopSelectedFeature(stopClickSelected?.feature ?? null);
  }, [stopClickSelected]);

  async function handleSaveStop(data: { name: string; transitRouteId: number }) {
    if (!pendingStop) return;
    setIsSavingStop(true);
    setStopFormError(null);

    try {
      const res = await createTransitStop(apiFetch, {
        name:           data.name,
        transitRouteId: data.transitRouteId,
        wktGeometry:    pendingStop.wkt,
      });

      if (!res.ok) {
        const message = res.status === 403 ? await res.text() : "Durak kaydedilemedi.";
        setStopFormError(message);
        onToast({ message, type: "error" });
        return;
      }

      const dto   = await res.json();
      const route = transitRoutes.find((r) => r.id === dto.transitRouteId);

      stopSourceRef.current.removeFeature(pendingStop.feature);
      stopSourceRef.current.addFeature(transitStopToFeature(dto, route?.color ?? "#3b82f6", route?.name ?? ""));

      setPendingStop(null);
      reloadTransitRoutes();
    } catch {
      setStopFormError("Sunucuya bağlanılamadı.");
      onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setIsSavingStop(false);
    }
  }

  function handleCancelStop() {
    if (pendingStop) stopSourceRef.current.removeFeature(pendingStop.feature);
    setPendingStop(null);
    setStopFormError(null);
  }

  function handleStopDeleted() {
    if (stopSelectedFeature) stopSourceRef.current.removeFeature(stopSelectedFeature);
    setStopSelectedFeature(null);
    clearStopSelected();
  }

  function handleAddStopToRoute(routeId: number) {
    setLockedRouteId(routeId);
    onDrawActiveChange(true);
    onToast({ message: "Haritada durağın ekleneceği noktayı işaretleyin.", type: "success" });
  }

  const canManageSelectedStop =
    !!stopSelectedFeature &&
    (roles.includes("Admin") ||
      (roles.includes("Ulaşım Operatörü") && stopSelectedFeature.get("stopUserId") === userId));

  return (
    <>
      {pendingStop && (
        <StopFormModal
          routes={transitRoutes}
          lockedRouteId={lockedRouteId}
          onSave={handleSaveStop}
          onCancel={handleCancelStop}
          isSaving={isSavingStop}
          error={stopFormError}
        />
      )}

      {!isPlainUser && stopSelectedFeature && (
        <StopInfoPopup
          feature={stopSelectedFeature}
          routes={transitRoutes}
          userLookup={userLookup}
          canManage={canManageSelectedStop}
          onClose={() => { setStopSelectedFeature(null); clearStopSelected(); }}
          onUpdated={() => {}}
          onDeleted={handleStopDeleted}
        />
      )}

      {routeManagementOpen && (
        <RouteManagementPanel
          routes={transitRoutes}
          reloadRoutes={reloadTransitRoutes}
          onClose={onRouteManagementClose}
          onAddStopToRoute={handleAddStopToRoute}
          routeLineLayer={routeLineLayer}
        />
      )}

      {vehicleSelected && (() => {
        const vehicleId = vehicleSelected.feature.get("vehicleId");
        const position   = vehiclePositions.get(vehicleId);
        if (!position) return null;

        return (
          <VehicleInfoPopup
            x={vehicleSelected.x}
            y={vehicleSelected.y}
            routeName={transitRoutes.find((r) => r.id === position.routeId)?.name ?? ""}
            plateNumber={position.plateNumber}
            progressPercentage={position.progressPercentage}
            onClose={clearVehicleSelected}
            onStopTracking={() => { stopTracking(); clearVehicleSelected(); }}
          />
        );
      })()}

      {routeSelected && (
        <RouteInfoPopup
          x={routeSelected.x}
          y={routeSelected.y}
          routeName={transitRoutes.find((r) => r.id === routeSelected.routeId)?.name ?? ""}
          canManage={canManageTransitRoutes}
          isSimulationRunning={selectedRouteRunning}
          isTrackingThisRoute={trackedRouteId === routeSelected.routeId}
          isBusy={routeActionBusy}
          onClose={clearRouteSelected}
          onStart={() => handleStartSimulation(routeSelected.routeId)}
          onStop={() => handleStopSimulation(routeSelected.routeId)}
          onTrack={() => startTracking(routeSelected.routeId)}
          onStopTracking={() => stopTracking()}
        />
      )}

      {vehicleModal && (
        <VehicleSelectionModal
          routeId={vehicleModal.routeId}
          mode={vehicleModal.mode}
          runningVehicleIds={modalRunningIds}
          onClose={() => setVehicleModal(null)}
          onConfirm={handleVehicleModalConfirm}
        />
      )}
    </>
  );
}