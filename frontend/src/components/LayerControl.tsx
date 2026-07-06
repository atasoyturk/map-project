import type { DrawingLayers } from "../hooks/useDrawing";

interface LayerControlProps {
  layers: DrawingLayers | null;
}

export function LayerControl({ layers }: LayerControlProps) {
  if (!layers) return null;

  const items = [
    { label: "Noktalar",   layer: layers.pointLayer   },
    { label: "Çizgiler",   layer: layers.lineLayer    },
    { label: "Poligonlar", layer: layers.polygonLayer },
  ];

  return (
    <div style={{
      position: "absolute", top: 70, right: 16, zIndex: 10,
      background: "rgba(255,255,255,0.95)", borderRadius: 10,
      padding: "10px 14px", boxShadow: "0 2px 12px rgba(0,0,0,0.12)",
      display: "flex", flexDirection: "column", gap: 8,
    }}>
      <span style={{ fontSize: 11, fontWeight: 600, color: "#64748b", letterSpacing: ".5px" }}>
        KATMANLAR
      </span>
      {items.map(({ label, layer }) => (
        <label key={label} style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }}>
          <input
            type="checkbox"
            defaultChecked
            onChange={(e) => layer.setVisible(e.target.checked)}
            style={{ cursor: "pointer" }}
          />
          <span style={{ fontSize: 13, color: "#374151" }}>{label}</span>
        </label>
      ))}
    </div>
  );
}