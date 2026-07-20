import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { UserGeoPermissionModal } from "../components/UserGeoPermissionModal";
import { getUsers } from "../api/userService";

interface UserDto {
  id:       number;
  email:    string;
  isActive: boolean;
  roles:    string[];
  teamId:   number | null;
  teamName: string | null;
}

const thStyle: React.CSSProperties = {
  padding: "10px 16px", textAlign: "left", fontSize: 12,
  fontWeight: 600, color: "#64748b", textTransform: "uppercase",
};

const tdStyle: React.CSSProperties = {
  padding: "12px 16px", color: "#374151", verticalAlign: "middle",
};

export function CompanyUserManagement() {
  const [users,        setUsers]        = useState<UserDto[]>([]);
  const [isLoading,    setIsLoading]    = useState(true);
  const [error,        setError]        = useState<string | null>(null);
  const [geoUserId,    setGeoUserId]    = useState<number | undefined>();
  const [geoUserEmail, setGeoUserEmail] = useState("");
  const [showGeoModal, setShowGeoModal] = useState(false);

  const { apiFetch } = useAuth();

  useEffect(() => {
    async function fetchUsers() {
      setIsLoading(true);
      try {
        const res = await getUsers(apiFetch);
        if (!res.ok) { setError("Kullanıcılar yüklenemedi."); return; }
        const data: UserDto[] = await res.json();
        // Sadece "Kullanıcı" rolündeki kişileri filtrele
        setUsers(data.filter((u) => u.roles.includes("Kullanıcı")));
      } catch {
        setError("Sunucuya bağlanılamadı.");
      } finally {
        setIsLoading(false);
      }
    }
    fetchUsers();
  }, [apiFetch]);

  return (
    <>
      <div>
        <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
          Kullanıcı Yönetimi
        </h1>
        <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
          Sisteme kayıtlı kullanıcıları ve coğrafi yetkilerini yönetin.
        </p>

        {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
        {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

        {!isLoading && !error && (
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>E-posta</th>
                  <th style={thStyle}>Rol</th>
                  <th style={thStyle}>İşlem</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                    <td style={tdStyle}>{user.email}</td>
                    <td style={tdStyle}>
                      <div style={{ display: "flex", gap: 4, flexWrap: "wrap" }}>
                        {user.roles.map((r) => (
                          <span key={r} style={{
                            padding: "2px 8px", borderRadius: 20,
                            background: "#eff6ff", color: "#3b82f6",
                            fontSize: 11, fontWeight: 500,
                          }}>
                            {r}
                          </span>
                        ))}
                      </div>
                    </td>
                    <td style={tdStyle}>
                      <button
                        onClick={() => {
                          setGeoUserId(user.id);
                          setGeoUserEmail(user.email);
                          setShowGeoModal(true);
                        }}
                        style={{
                          padding: "4px 10px", borderRadius: 6,
                          border: "1px solid rgba(16,185,129,.3)",
                          background: "rgba(16,185,129,.05)",
                          color: "#10b981", fontSize: 12, cursor: "pointer",
                        }}
                      >
                        Coğrafi Yetki
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {showGeoModal && geoUserId !== undefined && (
        <UserGeoPermissionModal
          userId={geoUserId}
          userEmail={geoUserEmail}
          userRoles={users.find((u) => u.id === geoUserId)?.roles ?? []}
          onClose={() => setShowGeoModal(false)}
        />
      )}
    </>
  );
}
