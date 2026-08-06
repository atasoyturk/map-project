import type { ApiFetch } from "../../features/map/core/api/geoService";

export interface IsochroneBandDto {
  minutesThreshold: number;
  polygonWkt:       string;
}

export interface IsochroneResultDto {
  bands: IsochroneBandDto[];
}

export function getIsochrone(
  apiFetch: ApiFetch, centerLon: number, centerLat: number, minutesThresholds: number[],
) {
  return apiFetch("/api/thematic-map/isochrone", {
    method: "POST",
    body:   JSON.stringify({ centerLon, centerLat, minutesThresholds }),
  });
}