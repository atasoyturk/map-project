import type { ApiFetch } from "../../map/core/api/geoService";

export function getUserPermissions(apiFetch: ApiFetch, userId: number) {
  return apiFetch(`/api/admin/users/${userId}/permissions`);
}

export function grantPermissionToUser(apiFetch: ApiFetch, userId: number, permissionId: number) {
  return apiFetch(`/api/admin/users/${userId}/permissions`, {
    method: "POST",
    body:   JSON.stringify({ permissionId }),
  });
}

export function revokePermissionFromUser(apiFetch: ApiFetch, userId: number, permissionId: number) {
  return apiFetch(`/api/admin/users/${userId}/permissions/${permissionId}`, { method: "DELETE" });
}