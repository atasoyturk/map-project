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

export interface OsmImportBbox {
  minLon: number;
  minLat: number;
  maxLon: number;
  maxLat: number;
}

// skipped = OSM POIs a user edited or deleted; skippedCategories = mapped categories that no longer exist.
export interface OsmImportResultDto {
  added:             number;
  updated:           number;
  skipped:           number;
  skippedCategories: number[];
}
