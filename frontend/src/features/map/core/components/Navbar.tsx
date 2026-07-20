import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth }    from "../../../auth/context/AuthContext";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { buildStyle } from "../../../../utils/mapStyle";
import { useDrawing, type DrawingLayers } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { useAnalysis } from "../hooks/useAnalysis";
import { AttributeModal } from "./AttributeModal";
import { Toast }      from "../../../../shared/components/Toast";
import type { DrawType, PendingGeometry } from "../types";
import { createGeoFeature, validateGeometry } from "../api/geoService";
import { useEdgeVisibility } from "../hooks/useEdgeVisibility";
import { BottomToolbar } from "./BottomToolbar";

interface NavbarProps {
  map:               Map | null;
  analysisActive:    boolean;
  onAnalysisChange:  (v: boolean) => void;
  activeType:        DrawType | null;
  onActiveTypeChange:(v: DrawType | null) => void;
  onLayersReady?:    (layers: DrawingLayers) => void;
  queryPanelOpen:    boolean;
  onQueryPanelToggle:() => void;
  layerControlOpen:  boolean;
  onLayerControlToggle: () => void;
  heatmapActive:    boolean;
  onHeatmapToggle:  () => void;
  poiDrawActive:    boolean;
  onPoiDrawChange:  (active: boolean) => void;
  poiFormOpen:      boolean;
  locAnalysisActive: boolean;
  onLocAnalysisToggle: () => void;
  onLocAnalysisPolygonReady?: (wkt: string, feature: any) => void;
}


interface ToastState {
  message: string;
  type:    "success" | "error";
}

export function Navbar({
  map,
  analysisActive,
  onAnalysisChange,
  activeType,
  onActiveTypeChange,
  onLayersReady,
  queryPanelOpen,
  onQueryPanelToggle,
  layerControlOpen,
  onLayerControlToggle,
  heatmapActive,
  onHeatmapToggle,
  poiDrawActive,
  onPoiDrawChange,
  poiFormOpen,
  locAnalysisActive,
  onLocAnalysisToggle,
  onLocAnalysisPolygonReady,
}: NavbarProps) {

  const [pendingGeometry, setPendingGeometry] = useState<PendingGeometry | null>(null);
  const [isSaving,        setIsSaving]        = useState(false);
  const [toast,           setToast]           = useState<ToastState | null>(null);
  const [analysisResult,  setAnalysisResult]  = useState<number | null>(null);
  const [teamModeActive,  setTeamModeActive]  = useState(false);

  const pointSourceRef   = useRef(new VectorSource());
  const lineSourceRef    = useRef(new VectorSource());
  const polygonSourceRef = useRef(new VectorSource());

  const { logout, apiFetch, roles, teamId } = useAuth();
  const navigate = useNavigate();

  const isPlainUser = roles.includes("User") && roles.length === 1;

  const canManagePoi  = roles.includes("Operator") || roles.includes("Admin");
  const isOperatorOnly = roles.includes("Operator") && !roles.includes("Admin");
  const otherToolActive = !!activeType || analysisActive;
  const toolsLocked = !!pendingGeometry || heatmapActive || poiDrawActive || poiFormOpen;

  useFeatureLoader({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    apiFetch,
    buildStyle,
    viewMode: teamModeActive ? "team" : "own",
  });

  const { layers } = useDrawing({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    activeType,
    onDrawEnd: async (pending) => {
      
      if (locAnalysisActive && pending.type === "Polygon") {
        // DashboardPage'e poligonu bildir (bu prop'u birazdan ekleyeceğiz)
        onLocAnalysisPolygonReady?.(pending.wkt, pending.feature);
        onActiveTypeChange(null);
        return;
      }
      
      pending.feature.setStyle(buildStyle("#3b82f6", ""));

      try {
        const res = await validateGeometry(apiFetch, pending.wkt);

        if (!res.ok) {
          const src =
            pending.type === "Point"      ? pointSourceRef.current   :
            pending.type === "LineString"  ? lineSourceRef.current    :
                                             polygonSourceRef.current;
          src.removeFeature(pending.feature);
          const message = await res.text();
          setToast({ message: `Hata: ${message}`, type: "error" });
          onActiveTypeChange(null);
          return;
        }

        setPendingGeometry(pending);
        onActiveTypeChange(null);
      } catch {
        setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
        onActiveTypeChange(null);
      }
    },
  });

  useEffect(() => {
    if (layers) onLayersReady?.(layers);
  }, [layers]);

  const { clear: clearAnalysis } = useAnalysis({
    map,
    active:   analysisActive,
    apiFetch,
    onResult: (count) => {
      setAnalysisResult(count);
      onAnalysisChange(false);
      setToast({
        message: `Analiz tamamlandı! Seçilen alan içinde ${count} adet öğe bulundu.`,
        type:    "success",
      });
    },
    onError: (msg) => setToast({ message: msg, type: "error" }),
  });

  async function handleModalSave(name: string, color: string) {
    if (!pendingGeometry) return;
    setIsSaving(true);
    try {
      const response = await createGeoFeature(apiFetch, pendingGeometry.type, {
        wktGeometry: pendingGeometry.wkt,
        name,
        color,
      });

      if (!response.ok) {
        if (response.status === 403) {
          const message = await response.text();
          setToast({ message: `Hata: ${message}`, type: "error" });
        } else {
          setToast({ message: "Kaydetme başarısız.", type: "error" });
        }
        const src =
          pendingGeometry.type === "Point"      ? pointSourceRef.current   :
          pendingGeometry.type === "LineString"  ? lineSourceRef.current    :
                                                   polygonSourceRef.current;
        src.removeFeature(pendingGeometry.feature);
        setPendingGeometry(null);
        return;
      }

      pendingGeometry.feature.setStyle(buildStyle(color, name));
      setToast({ message: "Başarıyla kaydedildi.", type: "success" });
      setPendingGeometry(null);
    } catch {
      setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setIsSaving(false);
    }
  }

  function handleSelect(type: DrawType) {
    setToast(null);
    onAnalysisChange(false);
    setAnalysisResult(null);
    clearAnalysis();
    onPoiDrawChange(false);
    onActiveTypeChange(activeType === type ? null : type);
  }

  function handleAnalysisToggle() {
    const next = !analysisActive;
    onAnalysisChange(next);
    onActiveTypeChange(null);
    onPoiDrawChange(false);
    if (!next) { clearAnalysis(); setAnalysisResult(null); }
    setToast(null);
  }

  function handlePoiButtonClick() {
    setToast(null);
    onAnalysisChange(false);
    setAnalysisResult(null);
    clearAnalysis();
    onActiveTypeChange(null);
    onPoiDrawChange(!poiDrawActive);
  }

  function handleModalCancel() {
    if (pendingGeometry) {
      const src =
        pendingGeometry.type === "Point"      ? pointSourceRef.current   :
        pendingGeometry.type === "LineString"  ? lineSourceRef.current    :
                                                 polygonSourceRef.current;
      src.removeFeature(pendingGeometry.feature);
    }
    setPendingGeometry(null);
    onActiveTypeChange(null);
  }

  function handleLogout() {
    pointSourceRef.current.clear();
    lineSourceRef.current.clear();
    polygonSourceRef.current.clear();
    logout();
    navigate("/login");
  }

  const isAdmin = roles.includes("Admin");

  const keepNavVisible = !isPlainUser && (
    !!pendingGeometry || !!activeType || analysisActive || heatmapActive ||
    poiDrawActive || poiFormOpen || locAnalysisActive || queryPanelOpen || layerControlOpen
  );

  const topVisible = useEdgeVisibility("top", keepNavVisible);

  if (isPlainUser) {
    return (
      <nav style={{
        position: "fixed", top: topVisible ? 0 : -60, left: 0, right: 0,
        height: 50,
        background: "#030c21",
        borderBottom: "1px solid rgba(255,255,255,.08)",
        display: "flex", alignItems: "center",
        justifyContent: "space-between",
        padding: "0 20px", zIndex: 1000,
        transition: "top .25s ease",
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            AtaGIS
          </span>
        </div>
        <button
          onClick={handleLogout}
          style={{
            padding: "6px 14px", borderRadius: 8,
            border: "1px solid rgba(255,255,255,.15)",
            background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500,
            cursor: "pointer", minWidth: 80,
          }}
        >
          Çıkış Yap
        </button>
      </nav>
    );
  }

  return (
    <>
      <nav style={{
        position: "fixed", top: topVisible ? 0 : -60, left: 0, right: 0,
        height: 50,
        background: "#030c21",
        borderBottom: "1px solid rgba(255,255,255,.08)",
        display: "flex", alignItems: "center",
        justifyContent: "space-between",
        padding: "0 20px", zIndex: 1000, gap: 12,
        transition: "top .25s ease",
      }}>

        <div style={{ display: "flex", alignItems: "center", gap: 12, minWidth: 120 }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
            <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
              AtaGIS
            </span>
          </div>
          {isAdmin && (
            <button
              onClick={() => navigate("/admin")}
              style={{
                padding:      "6px 14px",
                borderRadius: 8,
                border:       "1px solid rgba(255,255,255,.15)",
                background:   "transparent",
                color:        "#94a3b8",
                fontSize:     13,
                fontWeight:   500,
                cursor:       "pointer",
                transition:   "all .15s",
              }}
            >
              Panel
            </button>
          )}
        </div>

        <div style={{ display: "flex", alignItems: "center", gap: 8, left: "50%", position: "absolute", transform: "translateX(-50%)"}}>
          {!isOperatorOnly && (
            <>

              <button
                onClick={onQueryPanelToggle}
                disabled={toolsLocked}
                style={{
                  padding: "6px 14px", borderRadius: 8, border: "1px solid",
                  borderColor: queryPanelOpen ? "#6366f1" : "rgba(255,255,255,.15)",
                  background:  queryPanelOpen ? "rgba(99,102,241,.2)" : "transparent",
                  color:       queryPanelOpen ? "#a5b4fc" : "#94a3b8",
                  fontSize: 13, fontWeight: 500,
                  cursor:   toolsLocked ? "not-allowed" : "pointer",
                  opacity:  toolsLocked ? 0.5 : 1,
                  transition: "all .15s",
                }}
              >
                Geçmiş
              </button>

              <button
                onClick={handleAnalysisToggle}
                disabled={toolsLocked}
                style={{
                  padding: "6px 14px", borderRadius: 8, border: "1px solid",
                  borderColor: analysisActive ? "#eab308" : "rgba(255,255,255,.15)",
                  background:  analysisActive ? "rgba(234,179,8,.2)" : "transparent",
                  color:       analysisActive ? "#fde047" : "#94a3b8",
                  fontSize: 13, fontWeight: 500,
                  cursor:   toolsLocked ? "not-allowed" : "pointer",
                  opacity:  toolsLocked ? 0.5 : 1,
                  transition: "all .15s",
                }}
              >
                Alan Tara
              </button>

              <button
                onClick={onLocAnalysisToggle}
                disabled={toolsLocked}
                style={{
                  padding: "6px 14px", borderRadius: 8, border: "1px solid",
                  borderColor: locAnalysisActive ? "#ec4899" : "rgba(255,255,255,.15)",
                  background:  locAnalysisActive ? "rgba(236,72,153,.2)" : "transparent",
                  color:       locAnalysisActive ? "#fbcfe8" : "#94a3b8",
                  fontSize: 13, fontWeight: 500,
                  cursor:   toolsLocked ? "not-allowed" : "pointer",
                  opacity:  toolsLocked ? 0.5 : 1,
                  transition: "all .15s",
                }}
              >
                Konum Analizi
              </button>

              <button
                onClick={onLayerControlToggle}
                disabled={toolsLocked}
                style={{
                  padding: "6px 14px", borderRadius: 8, border: "1px solid",
                  borderColor: layerControlOpen ? "#3b82f6" : "rgba(255,255,255,.15)",
                  background:  layerControlOpen ? "rgba(59,130,246,.2)" : "transparent",
                  color:       layerControlOpen ? "#93c5fd" : "#94a3b8",
                  fontSize: 13, fontWeight: 500,
                  cursor:   toolsLocked ? "not-allowed" : "pointer",
                  opacity:  toolsLocked ? 0.5 : 1,
                  transition: "all .15s",
                }}
              >
                Filtre
              </button>

              {analysisResult !== null && (
                <button
                  onClick={() => { clearAnalysis(); setAnalysisResult(null); setToast(null); }}
                  style={{
                    padding: "6px 12px", borderRadius: 8,
                    border: "1px solid rgba(234,179,8,.4)",
                    background: "rgba(234,179,8,.1)",
                    color: "#fde047", fontSize: 12, cursor: "pointer",
                  }}
                >
                  Temizle
                </button>
              )}

              <button
                onClick={onHeatmapToggle}
                disabled={!!pendingGeometry || poiDrawActive || poiFormOpen}
                style={{
                  padding: "6px 14px", borderRadius: 8, border: "1px solid",
                  borderColor: heatmapActive ? "#ef4444" : "rgba(255,255,255,.15)",
                  background:  heatmapActive ? "rgba(239,68,68,.2)" : "transparent",
                  color:       heatmapActive ? "#fca5a5" : "#94a3b8",
                  fontSize: 13, fontWeight: 500,
                  cursor:   (pendingGeometry || poiDrawActive || poiFormOpen) ? "not-allowed" : "pointer",
                  opacity:  (pendingGeometry || poiDrawActive || poiFormOpen) ? 0.5 : 1,
                  transition: "all .15s",
                }}
              >
                Isı Haritası
              </button>

              {!isAdmin && teamId !== null && (
                <button
                  onClick={() => setTeamModeActive((p) => !p)}
                  disabled={toolsLocked}
                  style={{
                    padding: "6px 14px", borderRadius: 8, border: "1px solid",
                    borderColor: teamModeActive ? "#10b981" : "rgba(255,255,255,.15)",
                    background:  teamModeActive ? "rgba(16,185,129,.2)" : "transparent",
                    color:       teamModeActive ? "#6ee7b7" : "#94a3b8",
                    fontSize: 13, fontWeight: 500,
                    cursor:   toolsLocked ? "not-allowed" : "pointer",
                    opacity:  toolsLocked ? 0.5 : 1,
                    transition: "all .15s",
                  }}
                >
                  Takım Haritası
                </button>
              )}
            </>
          )}

          {canManagePoi && (
            <button
              onClick={handlePoiButtonClick}
              disabled={!!pendingGeometry || heatmapActive || otherToolActive || poiFormOpen}
              style={{
                padding: "6px 14px", borderRadius: 8, border: "1px solid",
                borderColor: poiDrawActive ? "#0d9488" : "rgba(255,255,255,.15)",
                background:  poiDrawActive ? "rgba(13,148,136,.2)" : "transparent",
                color:       poiDrawActive ? "#5eead4" : "#94a3b8",
                fontSize: 13, fontWeight: 500,
                cursor:   (pendingGeometry || heatmapActive || otherToolActive || poiFormOpen) ? "not-allowed" : "pointer",
                opacity:  (pendingGeometry || heatmapActive || otherToolActive || poiFormOpen) ? 0.5 : 1,
                transition: "all .15s",
              }}
            >
              POI
            </button>
          )}
        </div>

        <button
          onClick={handleLogout}
          style={{
            padding: "6px 14px", borderRadius: 8,
            border: "1px solid rgba(255,255,255,.15)",
            background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500,
            cursor: "pointer", minWidth: 80,
          }}
        >
          Çıkış Yap
        </button>
      </nav>

      {!isOperatorOnly && (
        <BottomToolbar
          activeType={activeType}
          onSelect={handleSelect}
          onCancel={() => { onActiveTypeChange(null); setToast(null); }}
          disabled={toolsLocked}
          keepVisible={keepNavVisible}
        />
      )}

      {pendingGeometry && (
        <AttributeModal
          pending={pendingGeometry}
          onSave={handleModalSave}
          onCancel={handleModalCancel}
          isSaving={isSaving}
        />
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </>
  );
}