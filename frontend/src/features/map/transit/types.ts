import type { Feature } from "ol";
import type { TransitStopResponseDto } from "../../../shared/api/transitTypes";

export interface TransitRouteDetailDto {
  id:               number;
  name:             string;
  color:            string;
  userId:           number | null;
  createdDate:      string;
  routeWktGeometry: string | null;
  stops:            TransitStopResponseDto[];
}

export interface PendingStop {
  wkt:     string;
  feature: Feature;
}