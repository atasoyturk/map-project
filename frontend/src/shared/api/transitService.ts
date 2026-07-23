import type { ApiFetch } from "../../features/map/core/api/geoService";
import type { TransitStopRequestDto, TransitRouteRequestDto } from "../../features/map/transit/types";

// --- Stops ---

export function getAllTransitStops(apiFetch: ApiFetch) {
  return apiFetch("/api/transit-stop");
}

export function createTransitStop(apiFetch: ApiFetch, data: TransitStopRequestDto) {
  return apiFetch("/api/transit-stop", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function updateTransitStop(apiFetch: ApiFetch, id: number, data: TransitStopRequestDto) {
  return apiFetch(`/api/transit-stop/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

export function deleteTransitStop(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/transit-stop/${id}`, { method: "DELETE" });
}

// --- Routes ---

export function getAllTransitRoutes(apiFetch: ApiFetch) {
  return apiFetch("/api/transit-route");
}

export function getTransitRouteDetail(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/transit-route/${id}`);
}

export function createTransitRoute(apiFetch: ApiFetch, data: TransitRouteRequestDto) {
  return apiFetch("/api/transit-route", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function updateTransitRoute(apiFetch: ApiFetch, id: number, data: TransitRouteRequestDto) {
  return apiFetch(`/api/transit-route/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

export function deleteTransitRoute(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/transit-route/${id}`, { method: "DELETE" });
}

export function reorderTransitStops(apiFetch: ApiFetch, routeId: number, stopIdsInOrder: number[]) {
  return apiFetch(`/api/transit-route/${routeId}/stops/reorder`, {
    method: "PUT",
    body:   JSON.stringify({ stopIdsInOrder }),
  });
}

export function generateTransitRoute(apiFetch: ApiFetch, routeId: number) {
  return apiFetch(`/api/transit-route/${routeId}/generate-route`, {
    method: "POST",
  });
}