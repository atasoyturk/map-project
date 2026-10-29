import type { ApiFetch } from "../../../../shared/api/apiFetch";
import type { CategoryCountDto } from "./thematicMapService";

/** Counts are cumulative: everything reachable within minutesThreshold. */
export interface LocationReportBandDto {
  minutesThreshold: number;
  polygonWkt:       string;
  areaSquareMeters: number;
  competitorCount:  number;
  complementCount:  number;
  stopCount:        number;
  complementCounts: CategoryCountDto[];
}

/** Straight-line distance from the report center. */
export interface NearbyPlaceDto {
  name:           string;
  distanceMeters: number;
}

export interface LocationScoreDto {
  total:             number;
  complementScore:   number;
  competitionScore:  number;
  transitScore:      number;
  complementWeight:  number;
  competitionWeight: number;
  transitWeight:     number;
  saturationRatio:   number | null;
}

export interface LocationReportDto {
  bands:              LocationReportBandDto[];
  nearestCompetitors: NearbyPlaceDto[];
  nearestStops:       NearbyPlaceDto[];
  score:              LocationScoreDto;
}

export function getLocationReport(
  apiFetch: ApiFetch, centerLon: number, centerLat: number,
  competitorCategoryId: number, complementCategoryIds: number[],
) {
  return apiFetch("/api/analysis/location-report", {
    method: "POST",
    body:   JSON.stringify({ centerLon, centerLat, competitorCategoryId, complementCategoryIds }),
  });
}
