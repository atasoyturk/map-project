import { useEffect, useRef, useState } from "react";
import Map from "ol/Map";
import Draw from "ol/interaction/Draw";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import type { DrawType } from "../types/drawing";
import { useAuth } from "../context/AuthContext";

interface DrawingPanelProps {
  map: Map | null;
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

export function DrawingPanel({ map }: DrawingPanelProps) {
  const [activeType, setActiveType] = useState<DrawType | null>(null);
  const [status, setStatus]         = useState<string | null>(null);

  const drawRef   = useRef<Draw | null>(null);
  const sourceRef = useRef<VectorSource>(new VectorSource());
  const layerRef  = useRef<VectorLayer<VectorSource> | null>(null);

  const { apiFetch } = useAuth();

  // vector layer
  useEffect(() => {
    if (!map) return;

    const layer = new VectorLayer({ source: sourceRef.current, zIndex: 1 });
    map.addLayer(layer);
    layerRef.current = layer;

    return () => {
      map.removeLayer(layer);  // cleanup
    };
  }, [map]);

  // drawing interaction
  useEffect(() => {
    if (!map) return;

    // conflict guard
    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!activeType) return;

    const draw = new Draw({ source: sourceRef.current, type: activeType });

    draw.on("drawend", (event) => {
      const geometry = event.feature.getGeometry();
      if (!geometry) return;

      // EPSG:3857 → EPSG:4326 projection transformation
      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");

      // Geometry → WKT
      const wkt = new WKT().writeGeometry(cloned);

      // Backend request
      const endpoint = ENDPOINT_MAP[activeType];
      apiFetch(endpoint, {
        method: "POST",
        body: JSON.stringify({ wktGeometry: wkt }),
      })
        .then((res) => {
          setStatus(res.ok ? "Kaydedildi." : "Kaydetme başarısız.");
        })
        .catch(() => setStatus("Sunucuya bağlanılamadı."));

      setActiveType(null);
    });

    map.addInteraction(draw);
    drawRef.current = draw;

    return () => {
      map.removeInteraction(draw);
      drawRef.current = null;
    };
  }, [map, activeType]);

  function handleSelect(type: DrawType) {
    setStatus(null);
    setActiveType((prev) => (prev === type ? null : type)); // toggle
  }

  function handleCancel() {
    setStatus(null);
    setActiveType(null);
  }

  return (
    <div
      style={{
        position: "absolute",
        top: 16,
        left: 16,
        zIndex: 10,
        background: "rgba(255,255,255,0.95)",
        borderRadius: 12,
        padding: "12px 16px",
        boxShadow: "0 2px 12px rgba(0,0,0,0.15)",
        display: "flex",
        flexDirection: "column",
        gap: 8,
        minWidth: 140,
      }}
    >
      <span style={{ fontSize: 11, fontWeight: 600, color: "#64748b", letterSpacing: ".5px" }}>
        ÇİZİM MODU
      </span>

      {(["Point", "LineString", "Polygon"] as DrawType[]).map((type) => (
        <button
          key={type}
          onClick={() => handleSelect(type)}
          style={{
            padding: "7px 12px",
            borderRadius: 8,
            border: "1px solid",
            borderColor: activeType === type ? "#2563eb" : "#e2e8f0",
            background: activeType === type ? "#eff6ff" : "#fff",
            color: activeType === type ? "#1d4ed8" : "#374151",
            fontSize: 13,
            fontWeight: 500,
            cursor: "pointer",
            textAlign: "left",
            transition: "all .15s",
          }}
        >
          {LABEL_MAP[type]}
        </button>
      ))}

      {activeType && (
        <button
          onClick={handleCancel}
          style={{
            padding: "6px 12px",
            borderRadius: 8,
            border: "1px solid #fecaca",
            background: "#fef2f2",
            color: "#dc2626",
            fontSize: 12,
            cursor: "pointer",
          }}
        >
          İptal
        </button>
      )}

      {status && (
        <p style={{ fontSize: 12, color: "#64748b", margin: 0 }}>{status}</p>
      )}
    </div>
  );
}