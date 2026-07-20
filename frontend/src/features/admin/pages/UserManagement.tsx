import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { PermissionMatrix } from "../components/PermissionMatrix";
import { RoleAssignModal }  from "../components/RoleAssignModal";
import { UserGeoPermissionModal } from "../components/UserGeoPermissionModal";
import { getUsers, setUserActive } from "../api/userService";

interface UserDto {
  id:       number;
  email:    string;
  isActive: boolean;
  roles:    string[];
  teamId:   number | null;
  teamName: string | null;
}

export function UserManagement() {
  const [users,          setUsers]          = useState<UserDto[]>([]);
  const [isLoading,      setIsLoading]      = useState(true);
  const [error,          setError]          = useState<string | null>(null);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [roleUserId, setRoleUserId] = useState<number | null>(null);
  const [roleUserRoles, setRoleUserRoles] = useState<string[]>([]);
  const [geoUserId,    setGeoUserId]    = useState<number | undefined>();
  const [geoUserEmail, setGeoUserEmail] = useState("");
  const [showGeoModal, setShowGeoModal] = useState(false);

  const { apiFetch } = useAuth();

  async function fetchUsers() {
    setIsLoading(true);
    try {
      const res = await getUsers(apiFetch);
      if (!res.ok) { setError("Kullanıcılar yüklenemedi."); return; }
      setUsers(await res.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchUsers(); }, []);

  async function toggleActive(user: UserDto) {
    try {
      await setUserActive(apiFetch, user.id, !user.isActive);
      fetchUsers();
    } catch { }
  }

  return (
    <>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
          Şirket Yönetimi
        </h1>
        <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
          Şirket çalışanlarını yönetin.
        </p>

        {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
        {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

        {!isLoading && !error && (
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>ID</th>
                  <th style={thStyle}>E-posta</th>
                  <th style={thStyle}>Roller</th>
                  <th style={thStyle}>Takım</th>
                  <th style={thStyle}>Durum</th>
                  <th style={thStyle}>İşlem</th>
                </tr>
              </thead>
              <tbody>
                {users
                  .filter((user) => !user.roles.includes("Kullanıcı"))
                  .map((user) => (
                  <tr key={user.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                    <td style={tdStyle}>{user.id}</td>
                    <td style={tdStyle}>{user.email}</td>
                    <td style={tdStyle}>
                      <div style={{ display: "flex", gap: 4, flexWrap: "wrap" }}>
                        {user.roles.map((r) => (
                          <span key={r} style={{
                            padding:      "2px 8px",
                            borderRadius: 20,
                            background:   "#eff6ff",
                            color:        "#3b82f6",
                            fontSize:     11,
                            fontWeight:   500,
                          }}>
                            {r}
                          </span>
                        ))}
                      </div>
                    </td>
                    <td style={tdStyle}>
                      {user.teamName ?? <span style={{ color: "#94a3b8" }}>—</span>}
                    </td>
                    <td style={tdStyle}>
                      <span style={{
                        padding:      "2px 8px",
                        borderRadius: 20,
                        background:   user.isActive ? "#f0fdf4" : "#fef2f2",
                        color:        user.isActive ? "#16a34a" : "#dc2626",
                        fontSize:     11,
                        fontWeight:   500,
                      }}>
                        {user.isActive ? "Aktif" : "Pasif"}
                      </span>
                    </td>
                    <td style={tdStyle}>
                      <div style={{ display: "flex", gap: 6, flexWrap: "wrap" }}>
                        <button
                          onClick={() => toggleActive(user)}
                          style={{
                            padding:      "4px 10px",
                            borderRadius: 6,
                            border:       "1px solid #e2e8f0",
                            background:   "#f8fafc",
                            color:        "#374151",
                            fontSize:     12,
                            cursor:       "pointer",
                          }}
                        >
                          {user.isActive ? "Pasif Yap" : "Aktif Yap"}
                        </button>
                      {!user.roles.includes("Admin") && (
                        <button
                          onClick={() => {
                            setRoleUserId(user.id);
                            setRoleUserRoles(user.roles);
                          }}
                          style={{
                            padding:      "4px 10px",
                            borderRadius: 6,
                            border:       "1px solid rgba(59,130,246,.3)",
                            background:   "rgba(59,130,246,.05)",
                            color:        "#3b82f6",
                            fontSize:     12,
                            cursor:       "pointer",
                          }}
                        >
                          Roller
                        </button>
                      )}
                        <button
                          onClick={() => setSelectedUserId(user.id)}
                          style={{
                            padding:      "4px 10px",
                            borderRadius: 6,
                            border:       "1px solid rgba(99,102,241,.3)",
                            background:   "rgba(99,102,241,.05)",
                            color:        "#6366f1",
                            fontSize:     12,
                            cursor:       "pointer",
                          }}
                        >
                          Yetkiler
                        </button>
                      
                        <button
                        onClick={() => {
                          setGeoUserId(user.id);
                          setGeoUserEmail(user.email);
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
                        Özel Coğrafi Yetki
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

      {selectedUserId !== null && (
        <PermissionMatrix
          userId={selectedUserId}
          onClose={() => setSelectedUserId(null)}
        />
      )}
      
      {roleUserId !== null && (
        <RoleAssignModal
          userId={roleUserId}
          currentRoles={roleUserRoles}
          onClose={() => setRoleUserId(null)}
          onUpdated={() => { fetchUsers(); setRoleUserId(null); }}
        />
      )}
      {showGeoModal && geoUserId !== undefined && (
        <UserGeoPermissionModal
          userId={geoUserId}
          userEmail={geoUserEmail}
          userRoles={users.find(u => u.id === geoUserId)?.roles ?? []}
          onClose={() => setShowGeoModal(false)}
        />
      )}
      
    </>
  );
}

const thStyle: React.CSSProperties = {
  padding:       "10px 16px",
  textAlign:     "left",
  fontSize:      11,
  fontWeight:    600,
  color:         "#64748b",
  letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};