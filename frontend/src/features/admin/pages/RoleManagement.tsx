import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { RoleGeoPermissionModal } from "../components/RoleGeoPermissionModal";

interface RoleDto {
  id:   number;
  name: string;
}

export function RoleManagement() {
  const [roles,     setRoles]     = useState<RoleDto[]>([]);
  const [newRole,   setNewRole]   = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving,  setIsSaving]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);
  const [geoRoleId,   setGeoRoleId]   = useState<number | undefined>();
  const [geoRoleName, setGeoRoleName] = useState("");
  const [showGeoModal,setShowGeoModal]= useState(false);

  const { apiFetch } = useAuth();

  async function fetchRoles() {
    setIsLoading(true);
    try {
      const res = await apiFetch("/api/admin/roles");
      if (!res.ok) { setError("Roller yüklenemedi."); return; }
      setRoles(await res.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchRoles(); }, []);

  async function handleCreate() {
    if (!newRole.trim()) return;
    setIsSaving(true);
    try {
      const res = await apiFetch("/api/admin/roles", {
        method: "POST",
        body:   JSON.stringify({ name: newRole.trim() }),
      });
      if (!res.ok) return;
      setNewRole("");
      fetchRoles();
    } catch { }
    finally { setIsSaving(false); }
  }

  async function handleDelete(id: number) {
    if (!confirm("Bu rolü silmek istediğinize emin misiniz?")) return;
    try {
      await apiFetch(`/api/admin/roles/${id}`, { method: "DELETE" });
      fetchRoles();
    } catch {  }
  }

  return (
    <>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
          Rol Yönetimi
        </h1>
        <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
          Sistem rollerini yönetin.
        </p>

        {/* add new role */}
        <div style={{
          background: "#ffffff", borderRadius: 12,
          border: "1px solid #e2e8f0", padding: 16,
          marginBottom: 24, display: "flex", gap: 8,
        }}>
          <input
            type="text"
            placeholder="Yeni rol adı..."
            value={newRole}
            onChange={(e) => setNewRole(e.target.value)}
            style={{
              flex: 1, padding: "8px 12px", borderRadius: 8,
              border: "1px solid #e2e8f0", fontSize: 13,
              color: "#0f172a", outline: "none",
            }}
          />
          <button
            onClick={handleCreate}
            disabled={isSaving || !newRole.trim()}
            style={{
              padding:      "8px 16px",
              borderRadius: 8,
              border:       "none",
              background:   "#0f172a",
              color:        "#94a3b8",
              fontSize:     13,
              fontWeight:   500,
              cursor:       isSaving ? "not-allowed" : "pointer",
              opacity:      isSaving ? 0.6 : 1,
            }}
          >
            Ekle
          </button>
        </div>

        {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
        {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

        {!isLoading && !error && (
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>ID</th>
                  <th style={thStyle}>Rol Adı</th>
                  <th style={thStyle}>İşlem</th>
                </tr>
              </thead>
              <tbody>
                {roles.map((role) => (
                  <tr key={role.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                    <td style={tdStyle}>{role.id}</td>
                    <td style={tdStyle}>{role.name}</td>
                    <td style={tdStyle}>
                      <div style={{ display: "flex", gap: 6 }}>
                        <button
                          onClick={() => handleDelete(role.id)}
                          style={{
                            padding:      "4px 10px",
                            borderRadius: 6,
                            border:       "1px solid rgba(239,68,68,.3)",
                            background:   "rgba(239,68,68,.05)",
                            color:        "#ef4444",
                            fontSize:     12,
                            cursor:       "pointer",
                          }}
                        >
                          Sil
                        </button>
                        <button
                          onClick={() => {
                            setGeoRoleId(role.id);
                            setGeoRoleName(role.name);
                            setShowGeoModal(true);
                            
                          }}
                          style={{
                            padding:      "4px 10px",
                            borderRadius: 6,
                            border:       "1px solid rgba(16,185,129,.3)",
                            background:   "rgba(16,185,129,.05)",
                            color:        "#10b981",
                            fontSize:     12,
                            cursor:       "pointer",
                          }}
                        >
                          Coğrafi Yetki
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {showGeoModal && geoRoleId !== undefined && (
        <RoleGeoPermissionModal
          roleId={geoRoleId}
          roleName={geoRoleName}
          onClose={() => setShowGeoModal(false)}
        />
      )}
    </>
  );
}

const thStyle: React.CSSProperties = {
  padding:    "10px 16px",
  textAlign:  "left",
  fontSize:   11,
  fontWeight: 600,
  color:      "#64748b",
  letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};