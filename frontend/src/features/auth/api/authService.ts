import type { ApiFetch } from "../../map/core/api/geoService";

interface AuthCredentials {
  email:    string;
  password: string;
}

export function login(apiFetch: ApiFetch, credentials: AuthCredentials) {
  return apiFetch("/api/auth/login", {
    method: "POST",
    body:   JSON.stringify(credentials),
  });
}

export function register(apiFetch: ApiFetch, credentials: AuthCredentials) {
  return apiFetch("/api/auth/register", {
    method: "POST",
    body:   JSON.stringify(credentials),
  });
}