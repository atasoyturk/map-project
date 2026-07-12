import { Routes, Route, NavLink, Navigate } from "react-router-dom";
import { useAuth } from "../../auth/context/AuthContext";
import { useNavigate } from "react-router-dom";
import { UserManagement } from "./UserManagement";
import { RoleManagement } from "./RoleManagement";
import { GeoPermissionPage } from "./GeoPermissionPage";


export function AdminPage() {
  const { logout } = useAuth();
  const navigate   = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  const navItems = [
    { to: "/admin/users", label: "Kullanıcı Yönetimi"},
    { to: "/admin/roles", label: "Rol Yönetimi"      },
    { to: "/admin/geo",   label: "Coğrafi Yetkiler"   },
  ];

  return (
    <div style={{ display: "flex", height: "100vh", background: "#f8fafc" }}>

      {/* Sidebar */}
      <aside style={{
        width:           240,
        background:      "#0f172a",
        display:         "flex",
        flexDirection:   "column",
        padding:         "24px 0",
        flexShrink:      0,
        position:        "fixed",
        top:             0,
        left:            0,
        bottom:          0,
        zIndex:          100,
      }}>
        {/* Brand */}
        <div style={{ padding: "0 20px 24px", borderBottom: "1px solid rgba(255,255,255,.08)" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
            <span style={{ color: "#f1f5f9", fontWeight: 700, fontSize: 14 }}>AtaGIS</span>
          </div>
          <span style={{ color: "#475569", fontSize: 11, marginTop: 4, display: "block" }}>
            Yönetim Paneli
          </span>
        </div>

        {/* Nav Items */}
        <nav style={{ flex: 1, padding: "16px 12px" }}>
          {navItems.map(({ to, label }) => (
            <NavLink
              key={to}
              to={to}
              style={({ isActive }) => ({
                display:       "flex",
                alignItems:    "center",
                gap:           10,
                padding:       "10px 12px",
                borderRadius:  8,
                marginBottom:  4,
                textDecoration:"none",
                background:    isActive ? "rgba(59,130,246,.15)" : "transparent",
                borderLeft:    isActive ? "3px solid #3b82f6" : "3px solid transparent",
                color:         isActive ? "#93c5fd" : "#94a3b8",
                fontSize:      13,
                fontWeight:    isActive ? 600 : 400,
                transition:    "all .15s",
              })}
            >
              <span>{label}</span>
            </NavLink>
          ))}
        </nav>

        {/* return map + exit */}
        <div style={{ padding: "16px 12px", borderTop: "1px solid rgba(255,255,255,.08)", display: "flex", flexDirection: "column", gap: 8 }}>
          <button
            onClick={() => navigate("/dashboard")}
            style={{
              padding:      "8px 12px",
              borderRadius: 8,
              border:       "1px solid rgba(255,255,255,.15)",
              background:   "transparent",
              color:        "#94a3b8",
              fontSize:     12,
              cursor:       "pointer",
              textAlign:    "left",
            }}
          >
            ← Haritaya Dön
          </button>
          <button
            onClick={handleLogout}
            style={{
              padding:      "8px 12px",
              borderRadius: 8,
              border:       "1px solid rgba(239,68,68,.3)",
              background:   "rgba(239,68,68,.05)",
              color:        "#fca5a5",
              fontSize:     12,
              cursor:       "pointer",
              textAlign:    "left",
            }}
          >
            Çıkış Yap
          </button>
        </div>
      </aside>

      {/* Content */}
      <main style={{ marginLeft: 240, flex: 1, padding: 32, overflowY: "auto" }}>
        <Routes>
          <Route index element={<Navigate to="users" replace />} />
          <Route path="users" element={<UserManagement />} />
          <Route path="roles" element={<RoleManagement />} />
          <Route path="geo" element={<GeoPermissionPage />} />
        </Routes>
      </main>
    </div>
  );
}