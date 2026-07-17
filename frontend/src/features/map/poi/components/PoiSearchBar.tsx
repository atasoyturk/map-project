import { useState, useMemo } from "react";
import type Map from "ol/Map";
import type { Feature } from "ol";
import type Point from "ol/geom/Point";

interface PoiSearchBarProps {
  map: Map | null;
  poiFeatures: Feature[];
}

const MAX_SUGGESTIONS = 6;

export function PoiSearchBar({ map, poiFeatures }: PoiSearchBarProps) {
  const [query, setQuery] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const suggestions = useMemo(() => {
    const trimmed = query.trim().toLowerCase();
    if (!trimmed) return [];

    return poiFeatures
      .filter((f) => {
        const name = (f.get("poiName") as string | undefined)?.toLowerCase() ?? "";
        return name.includes(trimmed);
      })
      .slice(0, MAX_SUGGESTIONS);
  }, [query, poiFeatures]);

  function handleSelect(feature: Feature) {
    const geometry = feature.getGeometry() as Point | undefined;
    if (!map || !geometry) return;

    map.getView().animate({
      center:   geometry.getCoordinates(),
      zoom:     17,
      duration: 500,
    });

    setQuery(feature.get("poiName") as string ?? "");
    setIsOpen(false);
  }

  if (poiFeatures.length === 0) return null;

  return (
    <div style={{ position: "absolute", top: 12, left: 12, zIndex: 10, width: 200 }}>
      <input
        type="text"
        value={query}
        onChange={(e) => { setQuery(e.target.value); setIsOpen(true); }}
        onFocus={() => setIsOpen(true)}
        onBlur={() => setTimeout(() => setIsOpen(false), 150)}
        placeholder="POI ara..."
        style={{
          width: "100%", boxSizing: "border-box",
          padding: "6px 10px", borderRadius: 8,
          border: "1px solid rgba(226,232,240,0.6)", fontSize: 12,
          color: "#0f172a", outline: "none",
          background: "rgba(255,255,255,0.55)",
          backdropFilter: "blur(4px)",
          boxShadow: "0 2px 8px rgba(0,0,0,0.08)",
        }}
      />

      {isOpen && suggestions.length > 0 && (
        <div style={{
          marginTop: 4, background: "rgba(255,255,255,0.85)", borderRadius: 8,
          backdropFilter: "blur(4px)",
          boxShadow: "0 4px 12px rgba(0,0,0,0.12)", overflow: "hidden",
        }}>
          {suggestions.map((f) => (
            <button
              key={f.get("poiId")}
              onMouseDown={() => handleSelect(f)}
              style={{
                display: "block", width: "100%", textAlign: "left",
                padding: "7px 10px", border: "none",
                borderBottom: "1px solid rgba(241,245,249,0.8)",
                background: "transparent", color: "#0f172a",
                fontSize: 12, cursor: "pointer",
              }}
            >
              {f.get("poiName") as string}
            </button>
          ))}
        </div>
      )}

      {isOpen && query.trim() && suggestions.length === 0 && (
        <div style={{
          marginTop: 4, background: "rgba(255,255,255,0.85)", borderRadius: 8,
          backdropFilter: "blur(4px)",
          boxShadow: "0 4px 12px rgba(0,0,0,0.12)",
          padding: "7px 10px", fontSize: 12, color: "#94a3b8",
        }}>
          Sonuç bulunamadı.
        </div>
      )}
    </div>
  );
}
