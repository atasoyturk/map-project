const BASE_URL = "http://localhost:5000";

type LogoutFn = () => void;

export function createApiFetch(logout: LogoutFn) {
  return async function apiFetch(
    path: string,
    options: RequestInit = {}
  ): Promise<Response> {
    const token = localStorage.getItem("token");

    const headers: HeadersInit = {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    };

    const response = await fetch(`${BASE_URL}${path}`, {
      ...options,
      headers,
    });

    if (response.status === 401) {
      logout();
    }

    return response;
  };
}