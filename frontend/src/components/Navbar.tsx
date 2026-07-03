import { useRef, useState } from "react";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { Style, Fill, Stroke, Circle, Text } from "ol/style";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useDrawing } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { AttributeModal } from "./AttributeModal";
import { Toast } from "./Toast";
import type { DrawType, PendingGeometry } from "../types/drawing";

interface NavbarProps { map: Map | null; }

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

function hexToRgba(hex: string, alpha: number): string {
  const r = parseInt(hex.slice(1, 3), 16);
  const g = parseInt(hex.slice(3, 5), 16);
  const b = parseInt(hex.slice(5, 7), 16);
  return `rgba(${r},${g},${b},${alpha})`;
}

function buildStyle(color: string, name: string): Style {
  const c = color || "#3b82f6";
  return new Style({
    fill:   new Fill({ color: hexToRgba(c, 0.2) }),
    stroke: new Stroke({ color: c, width: 2 }),
    image:  new Circle({
      radius: 6,
      fill:   new Fill({ color: c }),
      stroke: new Stroke({ color: "#ffffff", width: 2 }),
    }),
    text: name ? new Text({
      text:     name,
      font:     "bold 13px sans-serif",
      fill:     new Fill({ color: "#0f172a" }),
      stroke:   new Stroke({ color: "#ffffff", width: 3 }),
      offsetY:  -16,
      overflow: true,
    }) : undefined,
  });
}

export function Navbar({ map }: NavbarProps) {
  const [activeType,      setActiveType]      = useState<DrawType | null>(null);
  const [pendingGeometry, setPendingGeometry] = useState<PendingGeometry | null>(null);
  const [isSaving,        setIsSaving]        = useState(false);
  const [toast,           setToast]           = useState<ToastState | null>(null);

  const sourceRef = useRef(new VectorSource());

  const { logout, apiFetch } = useAuth();
  const navigate             = useNavigate();

  useFeatureLoader({ map, source: sourceRef.current, apiFetch, buildStyle });

  useDrawing({
    map,
    source:     sourceRef.current,
    activeType,
    onDrawEnd:  (pending) => {
      pending.feature.setStyle(buildStyle("#3b82f6", ""));
      setPendingGeometry(pending);
      setActiveType(null);
    },
  });

  async function handleModalSave(name: string, color: string) {
    if (!pendingGeometry) return;
    setIsSaving(true);

    try {
      const response = await apiFetch(ENDPOINT_MAP[pendingGeometry.type], {
        method: "POST",
        body:   JSON.stringify({ wktGeometry: pendingGeometry.wkt, name, color }),
      });

      if (!response.ok) {
        setToast({ message: "Kaydetme başarısız.", type: "error" });
        return;
      }

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
              onClick={() => { setToast(null); setActiveType((p) => p === type ? null : type); }}
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