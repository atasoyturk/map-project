import type { Feature } from "ol";

export interface PoiRequestDto {
  name:         string;
  workingHours: string;
  wktGeometry:  string;
  categoryId:   number;
}

export interface PoiResponseDto {
  id:           number;
  name:         string;
  workingHours: string;
  wktGeometry:  string;
  categoryId:   number;
  userId:       number;
  createdDate:  string;
}

export interface PendingPoi {
  wkt:     string;
  feature: Feature;
}