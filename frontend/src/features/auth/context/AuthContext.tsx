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
  permission?: string | string[];
}

interface AuthContextValue {
  token:    string | null;
  roles:    string[];
  permissions:   string[];
  hasPermission: (name: string) => boolean;
  teamId:   number | null;
  userId:   number | null;
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

function getPermissions(token: string | null): string[] {
  if (!token) return [];
  try {
    const raw = jwtDecode<TokenPayload>(token).permission;
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

function getUserId(token: string | null): number | null {
  if (!token) return null;
  try {
    const payload = jwtDecode<TokenPayload>(token);
    if (!payload.sub) return null;
    const parsed = parseInt(payload.sub, 10);
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
  const permissions = getPermissions(token);
  const teamId = getTeamId(token);
  const userId = getUserId(token);

  function login(newToken: string) {
    localStorage.setItem("token", newToken);
    setToken(newToken);
  }

  function logout() {
    localStorage.removeItem("token");
    setToken(null);
  }

  const apiFetch = useMemo(() => createApiFetch(logout), []);

  function hasPermission(name: string): boolean {
    return permissions.includes(name);
  }

  return (
    <AuthContext.Provider value={{ token, roles, permissions, hasPermission, teamId, userId, login, logout, apiFetch }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}
