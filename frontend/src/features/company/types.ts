// frontend/src/features/company/types.ts

export interface CompanyCategoryResponseDto {
  id:   number;
  name: string;
}

export interface CompanyResponseDto {
  id:                    number;
  name:                  string;
  companyCategoryId:     number;
  companyCategoryName:   string;
}

export interface VehicleResponseDto {
  id:          number;
  plateNumber: string;
  companyId:   number;
  companyName: string;
}

export interface CompanyStatsDto {
  companyId:              number;
  companyName:            string;
  vehicleCount:           number;
  routeCount:             number;
  completedShipmentCount: number;
}

export interface ShipmentRecordDto {
  id:              number;
  transitRouteId:  number;
  routeName:       string;
  vehicleId:       number;
  plateNumber:     string;
  companyName:     string;
  startedAtUtc:    string;
  completedAtUtc:  string;
}