import type { DrawingLayers } from "../hooks/useDrawing";

interface LayerControlProps {
  layers: DrawingLayers | null;
  visible : boolean;
}

export function LayerControl({ layers, visible }: LayerControlProps) {
  if (!layers || !visible) return null;

  const items = [
    { label: "Noktalar",   layer: layers.pointLayer   },
    { label: "Çizgiler",   layer: layers.lineLayer    },
    { label: "Poligonlar", layer: layers.polygonLayer },
  ];

  return (
    <div style={{
      position: "absolute", top: 80, right: 16, zIndex: 10,
      background: "rgba(3,12,33)", borderRadius: 10,
      padding: "10px 14px", boxShadow: "0 2px 12px rgba(0,0,0,0.12)",
      display: "flex", flexDirection: "column", gap: 8,
    }}>
      <span style={{ fontSize: 11, fontWeight: 600, color: "#64748b", letterSpacing: ".5px" }}>
        Filtre
      </span>
      {items.map(({ label, layer }) => (
        <label key={label} style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }}>
          <input
            type="checkbox"
            defaultChecked
            onChange={(e) => layer.setVisible(e.target.checked)}
            style={{ cursor: "pointer" }}
          />
          <span style={{ fontSize: 13, color: "#94a3b8" }}>{label}</span>
        </label>
      ))}
    </div>
  );
}