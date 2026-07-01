import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

export function DashboardPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <div className="text-center">
        <h1 className="text-2xl font-semibold text-slate-900 mb-4">
          Dashboard
        </h1>
        <button
          onClick={handleLogout}
          className="text-sm text-slate-500 hover:text-slate-900 underline"
        >
          Çıkış yap
        </button>
      </div>
    </div>
  );
}