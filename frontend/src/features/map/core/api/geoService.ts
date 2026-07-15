import type { DrawType } from "../types";

export type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

interface GeoFeatureData {
  wktGeometry: string;
  name:        string;
  color:       string;
}

const ENDPOINT_MAP: Record<DrawType, string> = {
  Point:      "/api/point",
  LineString: "/api/line",
  Polygon:    "/api/polygon",
};

/** pulls the geo shapes from WFS Proxy (viewMode: own/team). */
export function getGeoFeatures(apiFetch: ApiFetch, typeName: string, viewMode?: "own" | "team") {
  const query = viewMode === "team" ? "&viewMode=team" : "";
  return apiFetch(`/api/proxy/geoserver/wfs?typeName=${typeName}${query}`);
}

/** saves new geo shapes features */
export function createGeoFeature(apiFetch: ApiFetch, type: DrawType, data: GeoFeatureData) {
  return apiFetch(ENDPOINT_MAP[type], {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

/** updates new geo shapes features */
export function updateGeoFeature(apiFetch: ApiFetch, type: DrawType, id: number, data: GeoFeatureData) {
  return apiFetch(`${ENDPOINT_MAP[type]}/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

/** soft deletes new geo shapes features*/
export function deleteGeoFeature(apiFetch: ApiFetch, type: DrawType, id: number) {
  return apiFetch(`${ENDPOINT_MAP[type]}/${id}`, { method: "DELETE" });
}

/** validates new geo shapes whether they are in geofencing boundaries or not */
export function validateGeometry(apiFetch: ApiFetch, wktGeometry: string) {
  return apiFetch("/api/geo-permission/validate", {
    method: "POST",
    body:   JSON.stringify({ wktGeometry }),
  });
}