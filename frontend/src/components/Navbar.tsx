import { useEffect, useRef, useState } from "react";
import Map from "ol/Map";
import Draw from "ol/interaction/Draw";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import type { DrawType } from "../types/drawing";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

interface NavbarProps {
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

export function Navbar({ map }: NavbarProps) {
  const [activeType, setActiveType] = useState<DrawType | null>(null);
  const [status, setStatus]         = useState<string | null>(null);

  const drawRef   = useRef<Draw | null>(null);
  const sourceRef = useRef(new VectorSource());

  const { logout, apiFetch } = useAuth();
  const navigate             = useNavigate();

  // Vektör katmanı
  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: sourceRef.current, zIndex: 1 });
    map.addLayer(layer);
    return () => { map.removeLayer(layer); };
  }, [map]);

  // Draw interaction yönetimi
  useEffect(() => {
    if (!map) return;

    if (drawRef.current) {
      map.removeInteraction(drawRef.current);
      drawRef.current = null;
    }

    if (!activeType) return;

    const draw = new Draw({ source: sourceRef.current, type: activeType });

    draw.on("drawend", (event) => {
      const geometry = event.feature.getGeometry();
      if (!geometry) return;

      const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
      const wkt    = new WKT().writeGeometry(cloned);

      apiFetch(ENDPOINT_MAP[activeType], {
        method: "POST",
        body: JSON.stringify({ wktGeometry: wkt }),
      })
        .then((res) => setStatus(res.ok ? "✓ Kaydedildi" : "Kaydetme başarısız."))
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
    setActiveType((prev) => (prev === type ? null : type));
  }

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <nav
      style={{
        position: "fixed",
        top: 0, left: 0, right: 0,
        height: 56,
        background: "#0f172a",
        borderBottom: "1px solid rgba(255,255,255,.08)",
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        padding: "0 20px",
        zIndex: 1000,
        gap: 12,
      }}
    >
      {/* Brand */}
      <div style={{ display: "flex", alignItems: "center", gap: 8, minWidth: 120 }}>
        <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
        <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
          GisPortal
        </span>
      </div>

      {/* Çizim butonları */}
      <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
        {(["Point", "LineString", "Polygon"] as DrawType[]).map((type) => (
          <button
            key={type}
            onClick={() => handleSelect(type)}
            style={{
              padding: "6px 14px",
              borderRadius: 8,
              border: "1px solid",
              borderColor: activeType === type ? "#3b82f6" : "rgba(255,255,255,.15)",
              background: activeType === type ? "rgba(59,130,246,.2)" : "transparent",
              color: activeType === type ? "#93c5fd" : "#94a3b8",
              fontSize: 13,
              fontWeight: 500,
              cursor: "pointer",
              transition: "all .15s",
            }}
          >
            {LABEL_MAP[type]}
          </button>
        ))}

        {activeType && (
          <button
            onClick={() => { setActiveType(null); setStatus(null); }}
            style={{
              padding: "6px 12px",
              borderRadius: 8,
              border: "1px solid rgba(239,68,68,.4)",
              background: "rgba(239,68,68,.1)",
              color: "#fca5a5",
              fontSize: 12,
              cursor: "pointer",
            }}
          >
            İptal
          </button>
        )}

        {status && (
          <span style={{ fontSize: 12, color: status.startsWith("✓") ? "#86efac" : "#fca5a5" }}>
            {status}
          </span>
        )}
      </div>

      {/* Çıkış */}
      <button
        onClick={handleLogout}
        style={{
          padding: "6px 14px",
          borderRadius: 8,
          border: "1px solid rgba(255,255,255,.15)",
          background: "transparent",
          color: "#94a3b8",
          fontSize: 13,
          fontWeight: 500,
          cursor: "pointer",
          minWidth: 80,
        }}
      >
        Çıkış Yap
      </button>
    </nav>
  );
}