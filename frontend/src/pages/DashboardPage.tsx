import { useState }         from "react";
import { useNavigate }      from "react-router-dom";
import { useAuth }          from "../context/AuthContext";
import { MapView }          from "../components/MapView";
import { DrawingPanel }     from "../components/DrawingPanel";
import Map                  from "ol/Map";

export function DashboardPage() {
  const { logout }    = useAuth();
  const navigate      = useNavigate();
  const [map, setMap] = useState<Map | null>(null);   // map instance

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div style={{ position: "relative", width: "100vw", height: "100vh" }}>
      <MapView onMapReady={setMap} />

      {map && <DrawingPanel map={map} />}  

      <button
        onClick={handleLogout}
        style={{
          position: "absolute",
          top: 16,
          right: 16,
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
}