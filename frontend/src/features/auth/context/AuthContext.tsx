import {
  createContext,
  useContext,
  useState,
  useMemo,
  type ReactNode,
} from "react";
import { jwtDecode } from "jwt-decode";
import { createApiFetch } from "../../../shared/api/apiFetch";

interface TokenPayload {
  sub:   string;
  email: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string | string[];
  team_id?: string;
}

interface AuthContextValue {
  token:    string | null;
  roles:    string[];
  teamId:   number | null;
  login:    (token: string) => void;
  logout:   () => void;
  apiFetch: ReturnType<typeof createApiFetch>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function getRoles(token: string | null): string[] {
  if (!token) return [];
  try {
    const payload = jwtDecode<TokenPayload>(token);
    const raw = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    if (!raw) return [];
    return Array.isArray(raw) ? raw : [raw];
  } catch {
    return [];
  }
}

function getTeamId(token: string | null): number | null {
  if (!token) return null;
  try {
    const payload = jwtDecode<TokenPayload>(token);
    if (!payload.team_id) return null;
    const parsed = parseInt(payload.team_id, 10);
    return Number.isNaN(parsed) ? null : parsed;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );

  const roles  = getRoles(token);
  const teamId = getTeamId(token);

  function login(newToken: string) {
    localStorage.setItem("token", newToken);
    setToken(newToken);
  }

  function logout() {
    localStorage.removeItem("token");
    setToken(null);
  }

  const apiFetch = useMemo(() => createApiFetch(logout), []);

  return (
    <AuthContext.Provider value={{ token, roles, teamId, login, logout, apiFetch }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}