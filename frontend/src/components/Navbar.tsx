import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { buildStyle } from "../utils/mapStyle";
import { useDrawing, type DrawingLayers } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { useAnalysis } from "../hooks/useAnalysis";
import { AttributeModal } from "./AttributeModal";
import { Toast } from "./Toast";
import type { DrawType, PendingGeometry } from "../types/drawing";

interface NavbarProps {
  map:               Map | null;
  analysisActive:    boolean;
  onAnalysisChange:  (v: boolean) => void;
  activeType:        DrawType | null;
  onActiveTypeChange:(v: DrawType | null) => void;
  onLayersReady?:    (layers: DrawingLayers) => void;
  queryPanelOpen:    boolean;
  onQueryPanelToggle:() => void;
  layerControlOpen : boolean;
  onLayerControlToggle: () => void;
}

const ENDPOINT_MAP: Record<DrawType, string> = {
  Point:      "/api/point",
  LineString: "/api/line",
  Polygon:    "/api/polygon",
};

const LABEL_MAP: Record<DrawType, string> = {
  Point:      "Nokta",
  LineString: "Çizgi",
  Polygon:    "Poligon",
};

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
}: NavbarProps) {

  const [pendingGeometry, setPendingGeometry] = useState<PendingGeometry | null>(null);
  const [isSaving,        setIsSaving]        = useState(false);
  const [toast,           setToast]           = useState<ToastState | null>(null);
  const [analysisResult,  setAnalysisResult]  = useState<number | null>(null);

  const pointSourceRef   = useRef(new VectorSource());
  const lineSourceRef    = useRef(new VectorSource());
  const polygonSourceRef = useRef(new VectorSource());

  const { logout, apiFetch } = useAuth();
  const navigate             = useNavigate();

  useFeatureLoader({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    apiFetch,
    buildStyle,
  });

  const { layers } = useDrawing({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    activeType,
    onDrawEnd: (pending) => {
      pending.feature.setStyle(buildStyle("#3b82f6", ""));
      setPendingGeometry(pending);
      onActiveTypeChange(null);
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
      const response = await apiFetch(ENDPOINT_MAP[pendingGeometry.type], {
        method: "POST",
        body:   JSON.stringify({ wktGeometry: pendingGeometry.wkt, name, color }),
      });
      if (!response.ok) { setToast({ message: "Kaydetme başarısız.", type: "error" }); return; }

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
    onActiveTypeChange(activeType === type ? null : type);
  }

  function handleAnalysisToggle() {
    const next = !analysisActive;
    onAnalysisChange(next);
    onActiveTypeChange(null);
    if (!next) { clearAnalysis(); setAnalysisResult(null); }
    setToast(null);
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

  return (
    <>
      <nav style={{
        position: "fixed", top: 0, left: 0, right: 0,
        height: 50, 
        background: "#030c21",
        borderBottom: "1px solid rgba(255,255,255,.08)",
        display: "flex", alignItems: "center",
        justifyContent: "space-between",
        padding: "0 20px", zIndex: 1000, gap: 12,
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8, minWidth: 120 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            GisPortal
          </span>
        </div>

        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          {(["Point", "LineString", "Polygon"] as DrawType[]).map((type) => (
            <button
              key={type}
              onClick={() => handleSelect(type)}
              disabled={!!pendingGeometry}
              style={{
                padding: "6px 14px", borderRadius: 8, border: "1px solid",
                borderColor: activeType === type ? "#3b82f6" : "rgba(255,255,255,.15)",
                background:  activeType === type ? "rgba(6,18,52,.2)" : "transparent",
                color:       activeType === type ? "#93c5fd" : "#94a3b8",
                fontSize: 13, fontWeight: 500,
                cursor:   pendingGeometry ? "not-allowed" : "pointer",
                opacity:  pendingGeometry ? 0.5 : 1,
                transition: "all .15s",
              }}
            >
              {LABEL_MAP[type]}
            </button>
          ))}

          {activeType && !pendingGeometry && (
            <button
              onClick={() => { onActiveTypeChange(null); setToast(null); }}
              style={{
                padding: "6px 12px", borderRadius: 8,
                border: "1px solid rgba(239,68,68,.4)",
                background: "rgba(239,68,68,.1)",
                color: "#fca5a5", fontSize: 12, cursor: "pointer",
              }}
            >
              İptal
            </button>
          )}

          <div style={{ width: 1, height: 24, background: "rgba(255, 255, 255, 0.2)" }} />

          <button
            onClick={onQueryPanelToggle}
            disabled={!!pendingGeometry}
            style={{
              padding: "6px 14px", borderRadius: 8, border: "1px solid",
              borderColor: queryPanelOpen ? "#6366f1" : "rgba(255,255,255,.15)",
              background:  queryPanelOpen ? "rgba(99,102,241,.2)" : "transparent",
              color:       queryPanelOpen ? "#a5b4fc" : "#94a3b8",
              fontSize: 13, fontWeight: 500,
              cursor:   pendingGeometry ? "not-allowed" : "pointer",
              opacity:  pendingGeometry ? 0.5 : 1,
              transition: "all .15s",
            }}
          >
            Geçmiş
          </button>

          <button
            onClick={handleAnalysisToggle}
            disabled={!!pendingGeometry}
            style={{
              padding: "6px 14px", borderRadius: 8, border: "1px solid",
              borderColor: analysisActive ? "#eab308" : "rgba(255,255,255,.15)",
              background:  analysisActive ? "rgba(234,179,8,.2)" : "transparent",
              color:       analysisActive ? "#fde047" : "#94a3b8",
              fontSize: 13, fontWeight: 500,
              cursor:   pendingGeometry ? "not-allowed" : "pointer",
              opacity:  pendingGeometry ? 0.5 : 1,
              transition: "all .15s",
            }}
          >
            Alan Tara
          </button>

          <button
            onClick={onLayerControlToggle}
            disabled={!!pendingGeometry}
            style={{
              padding: "6px 14px", borderRadius: 8, border: "1px solid",
              borderColor: layerControlOpen ? "#3b82f6" : "rgba(255,255,255,.15)",
              background:  layerControlOpen ? "rgba(59,130,246,.2)" : "transparent",
              color:       layerControlOpen ? "#93c5fd" : "#94a3b8",
              fontSize: 13, fontWeight: 500,
              cursor:   pendingGeometry ? "not-allowed" : "pointer",
              opacity:  pendingGeometry ? 0.5 : 1,
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