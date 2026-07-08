import { useEffect, useRef, useState } from "react";
import Map                             from "ol/Map";
import View                            from "ol/View";
import TileLayer                       from "ol/layer/Tile";
import OSM                             from "ol/source/OSM";
import VectorLayer                     from "ol/layer/Vector";
import VectorSource                    from "ol/source/Vector";
import Draw                            from "ol/interaction/Draw";
import { fromLonLat }                  from "ol/proj";
import { WKT }                         from "ol/format";
import { Style, Fill, Stroke }         from "ol/style";
import { useAuth }                     from "../../auth/context/AuthContext";

interface GeoPermissionMapProps {
  onClose: () => void;
  onSaved: () => void;
}

const TURKEY_CENTER = fromLonLat([35.2433, 38.9637]);

export function GeoPermissionMap({ onClose, onSaved }: GeoPermissionMapProps) {
  const mapRef    = useRef<HTMLDivElement>(null);
  const mapObjRef = useRef<Map | null>(null);
  const sourceRef = useRef(new VectorSource());
  const drawRef   = useRef<Draw | null>(null);

  const [name,     setName]     = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);
  const [hasDrawn, setHasDrawn] = useState(false);

  const { apiFetch } = useAuth();

  useEffect(() => {
    if (!mapRef.current) return;

    const drawLayer = new VectorLayer({
      source: sourceRef.current,
      style: new Style({
        fill:   new Fill({ color: "rgba(59,130,246,0.15)" }),
        stroke: new Stroke({ color: "#3b82f6", width: 2 }),
      }),
    });

    const map = new Map({
      target: mapRef.current,
      layers: [new TileLayer({ source: new OSM() }), drawLayer],
      view:   new View({ center: TURKEY_CENTER, zoom: 6 }),
    });

    mapObjRef.current = map;

    const draw = new Draw({ source: sourceRef.current, type: "Polygon" });
    draw.on("drawend", () => setHasDrawn(true));
    map.addInteraction(draw);
    drawRef.current = draw;

    return () => {
      map.dispose();
      mapObjRef.current = null;
      drawRef.current   = null;
      sourceRef.current.clear();
    };
  }, []);

  function handleClear() {
    sourceRef.current.clear();
    setHasDrawn(false);
    setError(null);

    if (mapObjRef.current && drawRef.current) {
      mapObjRef.current.removeInteraction(drawRef.current);
      const draw = new Draw({ source: sourceRef.current, type: "Polygon" });
      draw.on("drawend", () => setHasDrawn(true));
      mapObjRef.current.addInteraction(draw);
      drawRef.current = draw;
    }
  }

  async function handleSave() {
    if (!name.trim())  { setError("Lütfen sınır adı girin."); return; }

    const features = sourceRef.current.getFeatures();
    if (features.length === 0) { setError("Lütfen bir sınır çizin."); return; }

    const geometry = features[0].getGeometry();
    if (!geometry) return;

    const cloned = geometry.clone().transform("EPSG:3857", "EPSG:4326");
    const wkt    = new WKT().writeGeometry(cloned);

    setIsSaving(true);
    setError(null);

    try {
      const res = await apiFetch("/api/geo-permission", {
        method: "POST",
        body:   JSON.stringify({ name: name.trim(), wktGeometry: wkt }),
      });

      if (!res.ok) { setError("Kaydedilemedi. Lütfen tekrar deneyin."); return; }

      onSaved();
      onClose();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 4000,
        background: "rgba(0,0,0,0.6)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "24px", width: "min(720px, 92vw)",
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
          display: "flex", flexDirection: "column", gap: 16,
        }}
      >
        <div>
          <h2 style={{ fontSize: 17, fontWeight: 600, color: "#0f172a", margin: 0 }}>
            Yeni Coğrafi Sınır Tanımla
          </h2>
          <p style={{ fontSize: 12, color: "#64748b", margin: "4px 0 0" }}>
            Haritada poligon çizerek sınır oluşturun.
          </p>
        </div>

        {/* boundary name */}
        <input
          type="text"
          placeholder="Sınır adı"
          value={name}
          onChange={(e) => setName(e.target.value)}
          style={{
            padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />

        {/* map */}
        <div
          ref={mapRef}
          style={{
            width: "100%", height: 400,
            borderRadius: 10, border: "1px solid #e2e8f0",
            overflow: "hidden",
          }}
        />

        {error && (
          <p style={{ fontSize: 12, color: "#dc2626", margin: 0 }}>{error}</p>
        )}

        <div style={{ display: "flex", gap: 8, justifyContent: "flex-end" }}>
          <button
            onClick={handleClear}
            style={{
              padding: "8px 16px", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, cursor: "pointer",
            }}
          >
            Temizle
          </button>
          <button
            onClick={onClose}
            style={{
              padding: "8px 16px", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={!hasDrawn || isSaving || !name.trim()}
            style={{
              padding: "8px 20px", borderRadius: 8, border: "none",
              background: !hasDrawn || isSaving || !name.trim() ? "#94a3b8" : "#0f172a",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: !hasDrawn || isSaving || !name.trim() ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Kaydediliyor..." : "Sınırı Kaydet"}
          </button>
        </div>
      </div>
    </div>
  );
}