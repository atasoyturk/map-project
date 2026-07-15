import type { ApiFetch } from "../../features/map/core/api/geoService";

export function getAllPois(apiFetch: ApiFetch) {
  return apiFetch("/api/poi");
}

export function getCategoryTree(apiFetch: ApiFetch) {
  return apiFetch("/api/poi-category/tree");
}