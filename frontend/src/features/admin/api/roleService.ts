import type { ApiFetch } from "../../map/core/api/geoService";

export function getRoles(apiFetch: ApiFetch) {
  return apiFetch("/api/admin/roles");
}

export function assignRoleToUser(apiFetch: ApiFetch, userId: number, roleId: number) {
  return apiFetch(`/api/admin/users/${userId}/roles`, {
    method: "POST",
    body:   JSON.stringify({ roleId }),
  });
}

export function removeRoleFromUser(apiFetch: ApiFetch, userId: number, roleId: number) {
  return apiFetch(`/api/admin/users/${userId}/roles/${roleId}`, { method: "DELETE" });
}