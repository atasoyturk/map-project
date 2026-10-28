import type { ApiFetch } from "./apiFetch";

export function getUserLookup(apiFetch: ApiFetch) {
  return apiFetch("/api/users/lookup");
}