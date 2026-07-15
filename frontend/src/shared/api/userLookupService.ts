import type { ApiFetch } from "../../features/map/core/api/geoService";

export function getUserLookup(apiFetch: ApiFetch) {
  return apiFetch("/api/users/lookup");
}