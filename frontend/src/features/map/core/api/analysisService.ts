import type { ApiFetch } from "./geoService";

/** Alan Tara */
export function analyzeTempInventory(apiFetch: ApiFetch, wktGeometry: string) {
  return apiFetch("/api/analysis/temp-inventory", {
    method: "POST",
    body:   JSON.stringify({ wktGeometry }),
  });
}

export function getLocationAnalysis(apiFetch: any, data: { wktGeometry: string; criteria: { categoryId: number; score: number }[] }) {
  return apiFetch("/api/analysis/location-analysis", {
    method: "POST",
    body: JSON.stringify(data),
  });
}
