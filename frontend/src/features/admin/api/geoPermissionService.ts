import type { ApiFetch } from "../../map/core/api/geoService";

export function getAllGeoPermissions(apiFetch: ApiFetch) {
  return apiFetch("/api/geo-permission");
}

export function getGeoPermissionsByUser(apiFetch: ApiFetch, userId: number) {
  return apiFetch(`/api/geo-permission/user/${userId}`);
}

export function getGeoPermissionsByRole(apiFetch: ApiFetch, roleId: number) {
  return apiFetch(`/api/geo-permission/role/${roleId}`);
}

export function assignGeoPermissionToUser(apiFetch: ApiFetch, userId: number, geoPermissionId: number) {
  return apiFetch(`/api/geo-permission/user/${userId}/${geoPermissionId}`, { method: "POST" });
}

export function removeGeoPermissionFromUser(apiFetch: ApiFetch, userId: number, geoPermissionId: number) {
  return apiFetch(`/api/geo-permission/user/${userId}/${geoPermissionId}`, { method: "DELETE" });
}

export function assignGeoPermissionToRole(apiFetch: ApiFetch, roleId: number, geoPermissionId: number) {
  return apiFetch(`/api/geo-permission/role/${roleId}/${geoPermissionId}`, { method: "POST" });
}

export function removeGeoPermissionFromRole(apiFetch: ApiFetch, roleId: number, geoPermissionId: number) {
  return apiFetch(`/api/geo-permission/role/${roleId}/${geoPermissionId}`, { method: "DELETE" });
}