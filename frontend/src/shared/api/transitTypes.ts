export interface TransitStopRequestDto {
  name:           string;
  wktGeometry:    string;
  transitRouteId: number;
}

export interface TransitStopResponseDto {
  id:             number;
  name:           string;
  wktGeometry:    string;
  transitRouteId: number;
  userId:         number;
  sortOrder:      number;
  createdDate:    string;
}

export interface TransitRouteRequestDto {
  name:  string;
  color: string;
}

export interface TransitRouteResponseDto {
  id:               number;
  name:             string;
  color:            string;
  userId:           number | null;
  createdDate:      string;
  routeWktGeometry: string | null;
}
