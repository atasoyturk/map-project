import type { ApiFetch } from "../../features/map/core/api/geoService";

export function getAllPois(apiFetch: ApiFetch) {
  return apiFetch("/api/poi");
}

export function getCategoryTree(apiFetch: ApiFetch) {
  return apiFetch("/api/poi-category/tree");
}

interface PoiPayload {
  name:         string;
  workingHours: string;
  categoryId:   number;
  wktGeometry:  string;
}

export function createPoi(apiFetch: ApiFetch, data: PoiPayload) {
  return apiFetch("/api/poi", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function updatePoi(apiFetch: ApiFetch, id: number, data: PoiPayload) {
  return apiFetch(`/api/poi/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

export function deletePoi(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/poi/${id}`, { method: "DELETE" });
}
