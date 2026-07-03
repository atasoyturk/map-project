import {
  createContext,
  useContext,
  useState,
  useMemo,
   type ReactNode,
} from "react";

import { createApiFetch } from "../api/apiFetch";

interface AuthContextValue {
  token: string | null;
  login: (token: string) => void;
  logout: () => void;
  apiFetch: ReturnType<typeof createApiFetch>;  
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );

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
    <AuthContext.Provider value={{ token, login, logout, apiFetch }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}