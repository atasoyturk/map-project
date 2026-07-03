import { useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { buildStyle } from "../utils/mapStyle"
import { useDrawing } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { useAnalysis } from "../hooks/useAnalysis";
import { AttributeModal } from "./AttributeModal";
import { Toast } from "./Toast";
import type { DrawType, PendingGeometry } from "../types/drawing";


interface NavbarProps {
  map:              Map | null;
  analysisActive:   boolean;           
  onAnalysisChange: (v: boolean) => void;
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

export function Navbar({ map, analysisActive, onAnalysisChange }: NavbarProps) {
  const [activeType,     setActiveType]     = useState<DrawType | null>(null);
  const [pendingGeometry,setPendingGeometry]= useState<PendingGeometry | null>(null);
  const [isSaving,       setIsSaving]       = useState(false);
  const [toast,          setToast]          = useState<ToastState | null>(null);
  const [analysisResult, setAnalysisResult] = useState<number | null>(null);

  const sourceRef = useRef(new VectorSource());
  const { logout, apiFetch } = useAuth();
  const navigate             = useNavigate();

  useFeatureLoader({ map, source: sourceRef.current, apiFetch, buildStyle });

  useDrawing({
    map,
    source:    sourceRef.current,
    activeType,
    onDrawEnd: (pending) => {
      pending.feature.setStyle(buildStyle("#3b82f6", ""));
      setPendingGeometry(pending);
      setActiveType(null);
    },
  });

  const { clear: clearAnalysis } = useAnalysis({
    map,
    active:   analysisActive,
    apiFetch,
    onResult: (count) => {
      setAnalysisResult(count);
      onAnalysisChange(false);
      setToast({
        message: `Analiz tamamlandı! Seçilen alan içinde ${count} adet envanter bulundu.`,
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

      let intersectedCount: number | null = null;
      if (pendingGeometry.type === "Polygon") {
        const data = await response.json();
        intersectedCount = data.intersectedInventoryCount;
      }
      pendingGeometry.feature.setStyle(buildStyle(color, name));
      setToast({
        message: intersectedCount !== null
          ? `Poligon kaydedildi! Alan içinde ${intersectedCount} envanter mevcut.`
          : "Başarıyla kaydedildi.",
        type: "success",
      });
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
    setActiveType((p) => p === type ? null : type);
  }

  function handleAnalysisToggle() {
    const next = !analysisActive;
    onAnalysisChange(next);        
    setActiveType(null);
    if (!next) { clearAnalysis(); setAnalysisResult(null); }
    setToast(null);
  }

  function handleModalCancel() {
    if (pendingGeometry) sourceRef.current.removeFeature(pendingGeometry.feature);
    setPendingGeometry(null);
    setActiveType(null);
  }

  function handleLogout() {
    sourceRef.current.clear();
    logout();
    navigate("/login");
  }

  return (
    <>
      <nav style={{
        position: "fixed", top: 0, left: 0, right: 0,
        height: 56, background: "#0f172a",
        borderBottom: "1px solid rgba(255,255,255,.08)",
        display: "flex", alignItems: "center",
        justifyContent: "space-between",
        padding: "0 20px", zIndex: 1000, gap: 12,
      }}>
        {/* Brand */}
        <div style={{ display: "flex", alignItems: "center", gap: 8, minWidth: 120 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            GisPortal
          </span>
        </div>

        {/* Araçlar */}
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>

          {/* Çizim butonları */}
          {(["Point", "LineString", "Polygon"] as DrawType[]).map((type) => (
            <button
              key={type}
              onClick={() => handleSelect(type)}
              disabled={!!pendingGeometry}
              style={{
                padding: "6px 14px", borderRadius: 8, border: "1px solid",
                borderColor: activeType === type ? "#3b82f6" : "rgba(255,255,255,.15)",
                background:  activeType === type ? "rgba(59,130,246,.2)" : "transparent",
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

          {/* Çizim iptal */}
          {activeType && !pendingGeometry && (
            <button
              onClick={() => { setActiveType(null); setToast(null); }}
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

          {/* Ayraç */}
          <div style={{ width: 1, height: 24, background: "rgba(255,255,255,.1)" }} />

          {/* Envanter Analizi */}
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
            Envanter Analizi
          </button>

          {/* Analizi Temizle */}
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
              Analizi Temizle
            </button>
          )}
        </div>

        {/* Çıkış */}
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