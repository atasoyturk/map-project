import type { ApiFetch } from "./geoService";

/** Alan Tara */
export function analyzeTempInventory(apiFetch: ApiFetch, wktGeometry: string) {
  return apiFetch("/api/analysis/temp-inventory", {
    method: "POST",
    body:   JSON.stringify({ wktGeometry }),
  });
}