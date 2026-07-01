import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import { MapView } from "../components/MapView";

export function DashboardPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div style={{ position: "relative", width: "100vw", height: "100vh" }}>
      <MapView />
      <button
        onClick={handleLogout}
        style={{
          position: "absolute",
          top: "16px",
          right: "16px",
          zIndex: 1000,
        }}
        className="bg-white text-slate-700 text-sm font-medium px-4 py-2 
                  rounded-lg shadow hover:bg-slate-50 transition-colors 
                  border border-slate-200"
      >
        Çıkış Yap
      </button>
    </div>
  );
  // without z-index, OL canvas could be above the logout button
}