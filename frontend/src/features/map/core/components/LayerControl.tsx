import type { DrawingLayers } from "../hooks/useDrawing";
import type VectorLayer from "ol/layer/Vector";
import type VectorSource from "ol/source/Vector";

interface LayerControlProps {
  layers:  DrawingLayers | null;
  poiLayer?: VectorLayer<VectorSource> | null;
  visible: boolean;
}

export function LayerControl({ layers, poiLayer, visible }: LayerControlProps) {
  if (!layers || !visible) return null;

  const items = [
    { label: "Nokta",   layer: layers.pointLayer   },
    { label: "Çizgi",   layer: layers.lineLayer    },
    { label: "Poligon", layer: layers.polygonLayer },
    ...(poiLayer ? [{ label: "POI", layer: poiLayer }] : []),
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
