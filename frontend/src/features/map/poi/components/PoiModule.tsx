import { useEffect, useRef, useState } from "react";
import OlMap from "ol/Map";
import type Feature from "ol/Feature";
import VectorSource from "ol/source/Vector";
import VectorLayer from "ol/layer/Vector";
import { usePoiLoader, poiToFeature } from "../hooks/usePoiLoader";
import { usePoiDraw } from "../hooks/usePoiDraw";
import { usePoiClick } from "../hooks/usePoiClick";
import { PoiFormModal } from "./PoiFormModal";
import { PoiInfoPopup } from "./PoiInfoPopup";
import { createPoi } from "../../../../shared/api/poiService";
import type { PoiResponseDto, PendingPoi } from "../types";
import type { CategoryTreeNode } from "../../../../shared/utils/categoryTree";
import type { UserLookupEntry } from "../../core/hooks/useUserLookup";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;
interface ToastState { message: string; type: "success" | "error"; }

interface PoiModuleProps {
  map:                OlMap | null;
  apiFetch:           ApiFetch;
  categories:         CategoryTreeNode[];
  userLookup:         Map<number, UserLookupEntry>;
  roles:              string[];
  userId:             number | null;
  isPlainUser:        boolean;
  otherFlowsIdle:     boolean;
  drawActive:         boolean;
  onDrawActiveChange: (active: boolean) => void;
  onFormOpenChange:   (open: boolean) => void;
  onFeaturesChange:   (features: Feature[]) => void;
  onLayerReady:       (layer: VectorLayer<VectorSource> | null) => void;
  onToast:            (toast: ToastState) => void;
}

export function PoiModule({
  map, apiFetch, categories, userLookup, roles, userId, isPlainUser,
  otherFlowsIdle, drawActive, onDrawActiveChange, onFormOpenChange,
  onFeaturesChange, onLayerReady, onToast,
}: PoiModuleProps) {
  const [pendingPoi,         setPendingPoi]         = useState<PendingPoi | null>(null);
  const [isSavingPoi,        setIsSavingPoi]        = useState(false);
  const [poiFormError,       setPoiFormError]       = useState<string | null>(null);
  const [poiSelectedFeature, setPoiSelectedFeature] = useState<Feature | null>(null);

  const poiSourceRef = useRef(new VectorSource());
  const poiLayerRef  = useRef<VectorLayer<VectorSource> | null>(null);

  const flowActive = drawActive || pendingPoi !== null;

  useEffect(() => { onFormOpenChange(pendingPoi !== null); }, [pendingPoi]);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: poiSourceRef.current, zIndex: 3 });
    map.addLayer(layer);
    poiLayerRef.current = layer;
    onLayerReady(layer);

    const handleSourceChange = () => onFeaturesChange(poiSourceRef.current.getFeatures());
    poiSourceRef.current.on("featuresloadend", handleSourceChange);
    poiSourceRef.current.on("addfeature", handleSourceChange);

    return () => {
      map.removeLayer(layer);
      poiLayerRef.current = null;
      onLayerReady(null);
      poiSourceRef.current.un("featuresloadend", handleSourceChange);
      poiSourceRef.current.un("addfeature", handleSourceChange);
    };
  }, [map]);

  usePoiLoader(map, poiSourceRef.current, apiFetch);

  usePoiDraw({
    map,
    source: poiSourceRef.current,
    active: drawActive,
    onDrawEnd: async (wkt, feature) => {
      onDrawActiveChange(false);

      try {
        const res = await apiFetch("/api/geo-permission/validate", {
          method: "POST",
          body:   JSON.stringify({ wktGeometry: wkt }),
        });

        if (!res.ok) {
          poiSourceRef.current.removeFeature(feature);
          const message = await res.text();
          onToast({ message: `Hata: ${message}`, type: "error" });
          return;
        }

        setPendingPoi({ wkt, feature });
      } catch {
        poiSourceRef.current.removeFeature(feature);
        onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
      }
    },
  });

  const { selected: poiClickSelected, clear: clearPoiSelected } = usePoiClick({
    map,
    poiLayer: poiLayerRef.current,
    enabled:  otherFlowsIdle && !flowActive && !isPlainUser,
  });

  useEffect(() => {
    setPoiSelectedFeature(poiClickSelected?.feature ?? null);
  }, [poiClickSelected]);

  async function handleSavePoi(data: { name: string; workingHours: string; categoryId: number }) {
    if (!pendingPoi) return;
    setIsSavingPoi(true);
    setPoiFormError(null);

    try {
      const res = await createPoi(apiFetch, {
        name:         data.name,
        workingHours: data.workingHours,
        categoryId:   data.categoryId,
        wktGeometry:  pendingPoi.wkt,
      });

      if (!res.ok) {
        const message = res.status === 403 ? await res.text() : "POI kaydedilemedi.";
        setPoiFormError(message);
        onToast({ message, type: "error" });
        return;
      }

      const dto: PoiResponseDto = await res.json();
      poiSourceRef.current.removeFeature(pendingPoi.feature);
      poiSourceRef.current.addFeature(poiToFeature(dto));
      setPendingPoi(null);
    } catch {
      setPoiFormError("Sunucuya bağlanılamadı.");
      onToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setIsSavingPoi(false);
    }
  }

  function handleCancelPoi() {
    if (pendingPoi) poiSourceRef.current.removeFeature(pendingPoi.feature);
    setPendingPoi(null);
    setPoiFormError(null);
  }

  function handlePoiUpdated() {
    onFeaturesChange([...poiSourceRef.current.getFeatures()]);
  }

  function handlePoiDeleted() {
    if (poiSelectedFeature) poiSourceRef.current.removeFeature(poiSelectedFeature);
    setPoiSelectedFeature(null);
    clearPoiSelected();
    onFeaturesChange([...poiSourceRef.current.getFeatures()]);
  }

  const canManageSelectedPoi =
    !!poiSelectedFeature &&
    (roles.includes("Admin") ||
      (roles.includes("POI Operatörü") && poiSelectedFeature.get("poiUserId") === userId));

  return (
    <>
      {pendingPoi && (
        <PoiFormModal
          categories={categories}
          onSave={handleSavePoi}
          onCancel={handleCancelPoi}
          isSaving={isSavingPoi}
          error={poiFormError}
        />
      )}

      {!isPlainUser && poiSelectedFeature && (
        <PoiInfoPopup
          feature={poiSelectedFeature}
          categories={categories}
          userLookup={userLookup}
          canManage={canManageSelectedPoi}
          onClose={() => { setPoiSelectedFeature(null); clearPoiSelected(); }}
          onUpdated={handlePoiUpdated}
          onDeleted={handlePoiDeleted}
        />
      )}
    </>
  );
}