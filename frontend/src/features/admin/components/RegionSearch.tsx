import { useState }                          from "react";
import { searchRegions, type NominatimResult }    from "../api/nominatim";

interface RegionSearchProps {
  onSelect: (geojson: NominatimResult["geojson"], displayName: string) => void;
}

export function RegionSearch({ onSelect }: RegionSearchProps) {
  const [query,       setQuery]       = useState("");
  const [results,     setResults]     = useState<NominatimResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [error,       setError]       = useState<string | null>(null);

  async function handleSearch() {
    if (!query.trim()) return;
    setIsSearching(true);
    setError(null);
    setResults([]);

    try {
      const found = await searchRegions(query);
      if (found.length === 0) {
        setError("Bu arama için sınır bulunamadı.");
        return;
      }
      setResults(found);
    } catch (e) {
      setError(e instanceof Error && e.message !== "Failed to fetch"
        ? e.message
        : "Nominatim servisine bağlanılamadı.");
    } finally {
      setIsSearching(false);
    }
  }

  function handleSelect(result: NominatimResult) {
    setResults([]);
    setError(null);
    onSelect(result.geojson, result.display_name);
  }

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
      <div style={{ display: "flex", gap: 8 }}>
        <input
          type="text"
          placeholder="Bölge adı (örn. Çankaya, Ankara)"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => { if (e.key === "Enter") handleSearch(); }}
          style={{
            flex: 1, padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />
        <button
          onClick={handleSearch}
          disabled={isSearching || !query.trim()}
          style={{
            padding: "8px 20px", borderRadius: 8, border: "none",
            background: isSearching || !query.trim() ? "#94a3b8" : "#0f172a",
            color: "#ffffff", fontSize: 13, fontWeight: 600,
            cursor: isSearching || !query.trim() ? "not-allowed" : "pointer",
          }}
        >
          {isSearching ? "Aranıyor..." : "Ara"}
        </button>
      </div>

      {results.length > 0 && (
        <div style={{
          border: "1px solid #e2e8f0", borderRadius: 8,
          maxHeight: 140, overflowY: "auto",
        }}>
          {results.map((r) => (
            <button
              key={r.place_id}
              onClick={() => handleSelect(r)}
              style={{
                display: "block", width: "100%", textAlign: "left",
                padding: "8px 12px", border: "none",
                borderBottom: "1px solid #f1f5f9",
                background: "#ffffff", color: "#0f172a",
                fontSize: 12, cursor: "pointer",
              }}
            >
              {r.display_name}
            </button>
          ))}
        </div>
      )}

      {error && (
        <p style={{ fontSize: 12, color: "#dc2626", margin: 0 }}>{error}</p>
      )}
    </div>
  );
}