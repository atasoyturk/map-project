import { useEffect, useRef, useState } from "react";
import OlMap from "ol/Map";
import type Feature from "ol/Feature";
import VectorSource from "ol/source/Vector";
import VectorLayer from "ol/layer/Vector";
import { useTransitStopLoader, transitStopToFeature } from "../hooks/useTransitStopLoader";
import { useTransitStopDraw } from "../hooks/useTransitStopDraw";
import { useTransitStopClick } from "../hooks/useTransitStopClick";
import { useTransitRoutes } from "../hooks/useTransitRoutes";
import { StopFormModal } from "./StopFormModal";
import { StopInfoPopup } from "./StopInfoPopup";
import { RouteManagementPanel } from "./RouteManagementPanel";
import { createTransitStop } from "../../../../shared/api/transitService";
import type { PendingStop } from "../types";
import type { UserLookupEntry } from "../../core/hooks/useUserLookup";

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

  const flowActive = drawActive || pendingStop !== null;

  useEffect(() => { onFormOpenChange(pendingStop !== null); }, [pendingStop]);

  // Bir durak ekleme akışı (kilitli ya da serbest) tamamen bittiğinde kilit sıfırlanır.
  useEffect(() => { if (!flowActive) setLockedRouteId(null); }, [flowActive]);

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

  useTransitStopLoader(map, stopSourceRef.current, apiFetch);
  const { routes: transitRoutes, reload: reloadTransitRoutes } = useTransitRoutes(apiFetch);

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
        />
      )}
    </>
  );
}