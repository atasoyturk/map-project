import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider }    from "./features/auth/context/AuthContext";
import { ProtectedRoute }  from "./features/auth/components/ProtectedRoute";
import { AdminRoute }      from "./features/auth/components/AdminRoute";
import { RegisterPage }    from "./features/auth/pages/RegisterPage";
import { LoginPage }       from "./features/auth/pages/LoginPage";
import { DashboardPage }   from "./features/map/pages/DashboardPage";
import { AdminPage }       from "./features/admin/pages/AdminPage";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login"    element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/*"
            element={
              <AdminRoute>
                <AdminPage />
              </AdminRoute>
            }
          />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}