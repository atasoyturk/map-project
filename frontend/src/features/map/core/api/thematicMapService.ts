import type { ApiFetch } from "../../../../shared/api/apiFetch";

export interface IsochroneBandDto {
  minutesThreshold: number;
  polygonWkt:       string;
}

export interface IsochroneResultDto {
  bands: IsochroneBandDto[];
}

export interface CategoryCountDto {
  categoryId:   number;
  categoryName: string;
  count:        number;
}

/** Counts are cumulative: everything reachable within minutesThreshold. */
export interface IsochroneAnalysisBandDto extends IsochroneBandDto {
  stopCount:      number;
  categoryCounts: CategoryCountDto[];
}

export interface IsochroneAnalysisResultDto {
  bands: IsochroneAnalysisBandDto[];
}

export function getIsochroneAnalysis(
  apiFetch: ApiFetch, centerLon: number, centerLat: number, minutesThresholds: number[],
) {
  return apiFetch("/api/analysis/isochrone", {
    method: "POST",
    body:   JSON.stringify({ centerLon, centerLat, minutesThresholds }),
  });
}

export function getIsochrone(
  apiFetch: ApiFetch, centerLon: number, centerLat: number, minutesThresholds: number[],
) {
  return apiFetch("/api/thematic-map/isochrone", {
    method: "POST",
    body:   JSON.stringify({ centerLon, centerLat, minutesThresholds }),
  });
}