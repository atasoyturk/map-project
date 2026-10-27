import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../context/AuthContext";

interface AdminRouteProps {
  children: ReactNode;
}

export function AdminRoute({ children }: AdminRouteProps) {
  const { token, hasPermission } = useAuth();

  if (!token)                    return <Navigate to="/login"     replace />;
  if (!hasPermission("admin_access")) return <Navigate to="/dashboard" replace />;

  return <>{children}</>;
}