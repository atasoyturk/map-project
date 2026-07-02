export type DrawType = "Point" | "LineString" | "Polygon";

export interface PendingGeometry {
  wkt:  string;
  type: DrawType;
}