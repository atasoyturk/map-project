import type { ApiFetch } from "../../../shared/api/apiFetch";

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