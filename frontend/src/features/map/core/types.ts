import type { Feature } from "ol";
import type { Geometry } from "ol/geom";

export type DrawType = "Point" | "LineString" | "Polygon";

export interface PendingGeometry {
  wkt:     string;
  type:    DrawType;
  feature: Feature<Geometry>;  
}