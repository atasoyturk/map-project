import type { ApiFetch } from "../../map/core/api/geoService";

export function getTeams(apiFetch: ApiFetch) {
  return apiFetch("/api/admin/teams");
}

export function createTeam(apiFetch: ApiFetch, name: string) {
  return apiFetch("/api/admin/teams", {
    method: "POST",
    body:   JSON.stringify({ name }),
  });
}

export function deleteTeam(apiFetch: ApiFetch, id: number) {
  return apiFetch(`/api/admin/teams/${id}`, { method: "DELETE" });
}