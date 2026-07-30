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

export const VehicleType = {
  Tir:    1,
  Otobus: 2,
} as const;

export type VehicleType = typeof VehicleType[keyof typeof VehicleType];

export const VEHICLE_TYPE_LABELS: Record<VehicleType, string> = {
  [VehicleType.Tir]:    "Tır",
  [VehicleType.Otobus]: "Otobüs",
};

export interface VehicleResponseDto {
  id:          number;
  plateNumber: string;
  companyId:   number;
  companyName: string;
  vehicleType: VehicleType;
}

export interface CompanyStatsDto {
  companyId:              number;
  companyName:            string;
  tirCount:               number;
  otobusCount:            number;
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