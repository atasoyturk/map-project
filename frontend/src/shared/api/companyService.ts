// frontend/src/shared/api/companyService.ts
import type { ApiFetch } from "../../features/map/core/api/geoService";

// --- Company Category ---

export function getAllCompanyCategories(apiFetch: ApiFetch) {
  return apiFetch("/api/company-category");
}

export function createCompanyCategory(apiFetch: ApiFetch, name: string) {
  return apiFetch("/api/company-category", {
    method: "POST",
    body:   JSON.stringify({ name }),
  });
}

export function deleteCompanyCategory(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/company-category/${id}`, { method: "DELETE" });
}

// --- Company ---

export function getAllCompanies(apiFetch: ApiFetch) {
  return apiFetch("/api/company");
}

export function createCompany(apiFetch: ApiFetch, data: { name: string; companyCategoryId: number }) {
  return apiFetch("/api/company", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function updateCompany(apiFetch: ApiFetch, id: number, data: { name: string; companyCategoryId: number }) {
  return apiFetch(`/api/company/${id}`, {
    method: "PUT",
    body:   JSON.stringify(data),
  });
}

export function deleteCompany(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/company/${id}`, { method: "DELETE" });
}

export function getRoutesByCompany(apiFetch: ApiFetch, companyId: number) {
  return apiFetch(`/api/company/${companyId}/routes`);
}

export function getCompaniesByRoute(apiFetch: ApiFetch, transitRouteId: number) {
  return apiFetch(`/api/company/by-route/${transitRouteId}`);
}

export function assignRouteToCompany(apiFetch: ApiFetch, companyId: number, transitRouteId: number) {
  return apiFetch(`/api/company/${companyId}/routes`, {
    method: "POST",
    body:   JSON.stringify({ transitRouteId }),
  });
}

export function removeRouteFromCompany(apiFetch: ApiFetch, companyId: number, transitRouteId: number) {
  return apiFetch(`/api/company/${companyId}/routes/${transitRouteId}`, { method: "DELETE" });
}

// --- Vehicle ---

export function getAllVehicles(apiFetch: ApiFetch) {
  return apiFetch("/api/vehicle");
}

export function getVehiclesByCompany(apiFetch: ApiFetch, companyId: number) {
  return apiFetch(`/api/vehicle/by-company/${companyId}`);
}

export function createVehicle(apiFetch: ApiFetch, data: { plateNumber: string; companyId: number }) {
  return apiFetch("/api/vehicle", {
    method: "POST",
    body:   JSON.stringify(data),
  });
}

export function deleteVehicle(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/vehicle/${id}`, { method: "DELETE" });
}

export function getCompanyStats(apiFetch: ApiFetch) {
  return apiFetch("/api/company/stats");
}

export function getShipmentRecords(apiFetch: ApiFetch, transitRouteId?: number) {
  const query = transitRouteId ? `?transitRouteId=${transitRouteId}` : "";
  return apiFetch(`/api/company/shipments${query}`);
}