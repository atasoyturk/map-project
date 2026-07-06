import { useState, useRef }  from "react";
import { WKT }               from "ol/format";
import type { Geometry }     from "ol/geom";
import type { SelectedFeatureInfo } from "../hooks/useSelect";
import { useAuth }           from "../context/AuthContext";
import { ConfirmModal } from "./ConfirmModal";


interface InfoPopupProps {
  info:      SelectedFeatureInfo;
  onClose:   () => void;
  onUpdated: (name: string, color: string) => void;
  onDelete:  () => void;
}

const ENDPOINT: Record<string, string> = {
  point:   "/api/point",
  line:    "/api/line",
  polygon: "/api/polygon",
};

const TYPE_LABEL: Record<string, string> = {
  point:   "Nokta",
  line:    "Çizgi",
  polygon: "Poligon",
};

export function InfoPopup({ info, onClose, onUpdated, onDelete }: InfoPopupProps) {
  const [name,     setName]     = useState(info.name);
  const [color,    setColor]    = useState(info.color || "#3b82f6");
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);

  // original geometry to restore if user cancels
  const originalGeomRef = useRef<Geometry | null>(
    info.feature.getGeometry()?.clone() ?? null
  );

  const { apiFetch } = useAuth();

  function handleCancel() {
    if (originalGeomRef.current) {
      info.feature.setGeometry(originalGeomRef.current);  // visual restore
    }
    onClose();
  }

  async function handleUpdate() {
    setIsSaving(true);
    setError(null);
    try {
      const geometry = info.feature.getGeometry();
      if (!geometry) return;

      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt    = new WKT().writeGeometry(cloned);

      const response = await apiFetch(`${ENDPOINT[info.type]}/${info.id}`, {
        method: "PUT",
        body:   JSON.stringify({ wktGeometry: wkt, name, color }),
      });

      if (!response.ok) { setError("Güncelleme başarısız."); return; }

      info.feature.set("name",  name);
      info.feature.set("color", color);
      onUpdated(name, color);
      onClose();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  const [isDeleting, setIsDeleting] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);


  async function handleDelete() {
    setIsDeleting(true);
    try {
      const response = await apiFetch(`${ENDPOINT[info.type]}/${info.id}`, {
        method: "DELETE",
      });
      if (!response.ok) { setError("Silme başarısız."); return; }
      onDelete();
      onClose();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsDeleting(false);
    }
  }

  return (
    <div
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
      onClick={handleCancel}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 340,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <span style={{ fontSize: 11, fontWeight: 600, color: "#6366f1", letterSpacing: ".8px", textTransform: "uppercase" }}>
          {TYPE_LABEL[info.type]} — ID: {info.id}
        </span>
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "4px 0 20px" }}>
          Öğe Durumu
        </h2>

        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            İsim
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            style={{
              width: "100%", boxSizing: "border-box",
              padding: "8px 12px", borderRadius: 8,
              border: "1px solid #e2e8f0", fontSize: 14,
              color: "#0f172a", outline: "none",
            }}
            onFocus={(e) => (e.target.style.borderColor = "#6366f1")}
            onBlur={(e)  => (e.target.style.borderColor = "#e2e8f0")}
          />
        </div>

        <div style={{ marginBottom: 24 }}>
          <label style={{ display: "block", fontSize: 12, fontWeight: 500, color: "#374151", marginBottom: 6 }}>
            Renk
          </label>
          <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
            <input
              type="color"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              style={{ width: 44, height: 36, borderRadius: 8, border: "1px solid #e2e8f0", cursor: "pointer", padding: 2 }}
            />
            <span style={{ fontSize: 13, color: "#64748b", fontFamily: "monospace" }}>
              {color.toUpperCase()}
            </span>
          </div>
        </div>

        {error && (
          <p style={{ fontSize: 12, color: "#dc2626", marginBottom: 12 }}>{error}</p>
        )}

        <div style={{ display: "flex", gap: 10, marginBottom: 10 }}>
          <button
            onClick={handleCancel}
            disabled={isSaving || isDeleting}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleUpdate}
            disabled={isSaving || isDeleting}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
              background: isSaving ? "#a5b4fc" : "linear-gradient(135deg,#4f46e5,#6366f1)",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: isSaving ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Güncelleniyor..." : "Güncelle"}
          </button>
        </div>

        <button
          onClick={() => setShowConfirm(true)}
          disabled={isDeleting || isSaving}
          style={{
            width: "100%", padding: "8px 0", borderRadius: 8,
            border: "1px solid rgba(239,68,68,.3)",
            background: "rgba(239,68,68,.05)",
            color: "#ef4444", fontSize: 12, fontWeight: 500,
            cursor: isDeleting ? "not-allowed" : "pointer",
            opacity: isDeleting ? 0.6 : 1,
          }}
        >
          {isDeleting ? "Siliniyor..." : "Sil"}
        </button>
      </div>
      {showConfirm && (
        <ConfirmModal
          message="Bu öğeyi silmek istediğinize emin misiniz?"
          onConfirm={() => { setShowConfirm(false); handleDelete(); }}
          onCancel={() => setShowConfirm(false)}
        />
      )}
    </div>
  );
}