import type { SelectedFeatureInfo } from "../hooks/useSelect";

interface FeaturePickerModalProps {
  features: SelectedFeatureInfo[];
  onPick:   (info: SelectedFeatureInfo) => void;
  onClose:  () => void;
}

const TYPE_LABEL: Record<string, string> = {
  point:   "Nokta",
  line:    "Çizgi",
  polygon: "Poligon",
};

export function FeaturePickerModal({ features, onPick, onClose }: FeaturePickerModalProps) {
  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "24px 28px", width: 320,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", margin: "0 0 6px" }}>
          Birden Fazla Obje Bulundu
        </h2>
        <p style={{ fontSize: 12, color: "#64748b", margin: "0 0 16px" }}>
          Hangi objeyi incelemek istersiniz?
        </p>

        <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
          {features.map((f) => (
            <button
              key={f.id}
              onClick={() => onPick(f)}
              style={{
                display: "flex", alignItems: "center", gap: 10,
                padding: "10px 14px", borderRadius: 10,
                border: "1px solid #e2e8f0", background: "#f8fafc",
                cursor: "pointer", textAlign: "left",
              }}
            >
              <div style={{
                width: 10, height: 10, borderRadius: "50%",
                background: f.color || "#3b82f6", flexShrink: 0,
              }} />
              <div>
                <div style={{ fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                  {f.name || "(İsimsiz)"}
                </div>
                <div style={{ fontSize: 11, color: "#94a3b8" }}>
                  {TYPE_LABEL[f.type]} — ID: {f.id}
                </div>
              </div>
            </button>
          ))}
        </div>

        <button
          onClick={onClose}
          style={{
            width: "100%", marginTop: 14, padding: "8px 0",
            borderRadius: 8, border: "1px solid #e2e8f0",
            background: "transparent", color: "#64748b",
            fontSize: 12, cursor: "pointer",
          }}
        >
          İptal
        </button>
      </div>
    </div>
  );
}