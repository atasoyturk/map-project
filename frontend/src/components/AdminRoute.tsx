import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../context/AuthContext";

interface AdminRouteProps {
  children: ReactNode;
}

export function AdminRoute({ children }: AdminRouteProps) {
  const { token, roles } = useAuth();

  if (!token)                    return <Navigate to="/login"     replace />;
  if (!roles.includes("Admin"))  return <Navigate to="/dashboard" replace />;

  return <>{children}</>;
}