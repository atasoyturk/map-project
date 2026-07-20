import type { ApiFetch } from "../../map/core/api/geoService";

export function getUsers(apiFetch: ApiFetch) {
  return apiFetch("/api/admin/users");
}

export function setUserActive(apiFetch: ApiFetch, userId: number, isActive: boolean) {
  return apiFetch(`/api/admin/users/${userId}/active`, {
    method: "PUT",
    body:   JSON.stringify({ isActive }),
  });
}

export function assignTeamToUser(apiFetch: ApiFetch, userId: number, teamId: number | null) {
  return apiFetch(`/api/admin/users/${userId}/team`, {
    method: "PUT",
    body:   JSON.stringify({ teamId }),
  });
}

export function assignTeamToUsers(apiFetch: ApiFetch, userIds: number[], teamId: number | null) {
  return apiFetch(`/api/admin/users/team/bulk`, {
    method: "PUT",
    body:   JSON.stringify({ userIds, teamId }),
  });
}