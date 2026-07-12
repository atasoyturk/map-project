export interface NominatimResult {
  place_id:     number;
  display_name: string;
  geojson: {
    type:        string;
    coordinates: unknown;
  };
}

const NOMINATIM_URL = "https://nominatim.openstreetmap.org/search";

/** Searches regions on Nominatim; returns only results with polygonal boundaries. */
export async function searchRegions(query: string): Promise<NominatimResult[]> {
  const url = `${NOMINATIM_URL}?format=jsonv2&polygon_geojson=1&limit=5&accept-language=tr&q=${encodeURIComponent(query.trim())}`;

  const res = await fetch(url);
  if (!res.ok) throw new Error("Arama başarısız oldu.");

  const data: NominatimResult[] = await res.json();
  return data.filter(
    (r) => r.geojson && (r.geojson.type === "Polygon" || r.geojson.type === "MultiPolygon")
  );
}