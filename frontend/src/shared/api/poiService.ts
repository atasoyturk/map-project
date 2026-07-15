import type { ApiFetch } from "../../features/map/core/api/geoService";

interface PoiPayload {
  name:         string;
  workingHours: string;
  categoryId:   number;
  wktGeometry:  string;
}

export function getAllPois(apiFetch: ApiFetch) {
  return apiFetch("/api/poi");
}

export function getCategoryTree(apiFetch: ApiFetch) {
  return apiFetch("/api/poi-category/tree");
}

export function createPoi(apiFetch: ApiFetch, data: PoiPayload) {
  return apiFetch("/api/poi", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}