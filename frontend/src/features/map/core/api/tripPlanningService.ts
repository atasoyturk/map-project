import type { ApiFetch } from "../../../../shared/api/apiFetch";

export interface TripLegDto {
  type:              "Walk" | "Bus";
  durationSeconds:   number;
  distanceMeters:    number;
  fromName:          string;
  toName:            string;
  pathWkt:           string;
  routeId:           number | null;
  routeName:         string | null;
  routeColor:        string | null;
}

export interface TripPlanResultDto {
  totalDurationSeconds: number;
  totalWalkingMeters:   number;
  legs:                 TripLegDto[];
}

export function planTrip(
  apiFetch: ApiFetch,
  startLon: number, startLat: number, endLon: number, endLat: number,
) {
  return apiFetch("/api/trip-planning/plan", {
    method: "POST",
    body:   JSON.stringify({ startLon, startLat, endLon, endLat }),
  });
}