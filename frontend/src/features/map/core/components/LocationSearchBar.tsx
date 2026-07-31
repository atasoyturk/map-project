import { useState } from "react";
import type OlMap from "ol/Map";
import { fromLonLat } from "ol/proj";
import { searchAddress, type AddressSearchResult } from "../../../admin/api/nominatim";

interface LocationSearchBarProps {
  map: OlMap | null;
  onLocationSelected: (lon: number, lat: number) => void;
  active?: boolean;
}

export function LocationSearchBar({ map, onLocationSelected, active }: LocationSearchBarProps) {
  const [query,       setQuery]       = useState("");
  const [results,     setResults]     = useState<AddressSearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [error,       setError]       = useState<string | null>(null);

  async function handleSearch() {
    if (!query.trim()) return;
    setIsSearching(true);
    setError(null);
    setResults([]);

    try {
      const found = await searchAddress(query);
      if (found.length === 0) {
        setError("Sonuç bulunamadı.");
        return;
      }
      setResults(found);
    } catch (e) {
      setError(e instanceof Error && e.message !== "Failed to fetch"
        ? e.message
        : "Arama servisine bağlanılamadı.");
    } finally {
      setIsSearching(false);
    }
  }

  function handleSelect(result: AddressSearchResult) {
    setResults([]);
    setError(null);
    setQuery(result.display_name);

    const lon = Number(result.lon);
    const lat = Number(result.lat);

    if (map) {
      map.getView().animate({
        center:   fromLonLat([lon, lat]),
        zoom:     17,
        duration: 500,
      });
    }

    onLocationSelected(lon, lat);
  }

  return (
    <div style={{ position: "absolute", bottom: 12, left: 12, zIndex: 10, width: 220 }}>
      <div style={{ display: "flex", gap: 6 }}>
        <input
          type="text"
          placeholder="Adres veya konum ara..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => { if (e.key === "Enter") handleSearch(); }}
          style={{
            flex: 1, boxSizing: "border-box",
            padding: "6px 10px", borderRadius: 8,
            border: active ? "2px solid #22c55e" : "1px solid rgba(226,232,240,0.6)", fontSize: 12,
            color: "#0f172a", outline: "none",
            background: "rgba(255,255,255,0.55)",
            backdropFilter: "blur(4px)",
            boxShadow: active ? "0 0 0 3px rgba(34,197,94,0.15)" : "0 2px 8px rgba(0,0,0,0.08)",
          }}
        />
        <button
          onClick={handleSearch}
          disabled={isSearching || !query.trim()}
          style={{
            padding: "6px 12px", borderRadius: 8,
            border: "1px solid rgba(226,232,240,0.6)", fontSize: 12, fontWeight: 600,
            color: isSearching || !query.trim() ? "#94a3b8" : "#0f172a",
            background: "rgba(255,255,255,0.55)",
            backdropFilter: "blur(4px)",
            boxShadow: "0 2px 8px rgba(0,0,0,0.08)",
            cursor: isSearching || !query.trim() ? "not-allowed" : "pointer",
          }}
        >
          {isSearching ? "..." : "Ara"}
        </button>
      </div>

      {error && (
        <div style={{
          marginTop: 4, background: "rgba(255,255,255,0.85)", borderRadius: 8,
          backdropFilter: "blur(4px)",
          boxShadow: "0 4px 12px rgba(0,0,0,0.12)",
          padding: "7px 10px", fontSize: 12, color: "#94a3b8",
        }}>
          {error}
        </div>
      )}

      {results.length > 0 && (
        <div style={{
          marginTop: 4, background: "rgba(255,255,255,0.85)", borderRadius: 8,
          backdropFilter: "blur(4px)",
          boxShadow: "0 4px 12px rgba(0,0,0,0.12)", overflow: "hidden",
          maxHeight: 180, overflowY: "auto",
        }}>
          {results.map((r) => (
            <button
              key={r.place_id}
              onClick={() => handleSelect(r)}
              style={{
                display: "block", width: "100%", textAlign: "left",
                padding: "7px 10px", border: "none",
                borderBottom: "1px solid rgba(241,245,249,0.8)",
                background: "transparent", color: "#0f172a",
                fontSize: 12, cursor: "pointer",
              }}
            >
              {r.display_name}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}