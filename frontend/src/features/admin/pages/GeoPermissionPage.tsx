import { useState, useEffect } from "react";
import { useAuth }             from "../../auth/context/AuthContext";
import { GeoPermissionMap }    from "../components/GeoPermissionMap";

interface GeoPermissionDto {
  id:          number;
  userId:      number | null;
  roleId:      number | null;
  wktGeometry: string;
  isActive:    boolean;
}

export function GeoPermissionPage() {
  const [permissions, setPermissions] = useState<GeoPermissionDto[]>([]);
  const [isLoading,   setIsLoading]   = useState(true);
  const [showMap,     setShowMap]     = useState(false);
  const [mapUserId,   setMapUserId]   = useState<number | undefined>();
  const [mapRoleId,   setMapRoleId]   = useState<number | undefined>();
  const [mapLabel,    setMapLabel]    = useState("");

  const { apiFetch } = useAuth();

  async function fetchPermissions() {
    setIsLoading(true);
    try {
      const res = await apiFetch("/api/geo-permission");
      if (!res.ok) return;
      setPermissions(await res.json());
    } catch {  }
    finally { setIsLoading(false); }
  }

  useEffect(() => { fetchPermissions(); }, []);

  async function handleDelete(id: number) {
    if (!confirm("Bu coğrafi yetki sınırını silmek istediğinize emin misiniz?")) return;
    try {
      await apiFetch(`/api/geo-permission/${id}`, { method: "DELETE" });
      fetchPermissions();
    } catch {  }
  }

  function openMap(userId?: number, roleId?: number, label?: string) {
    setMapUserId(userId);
    setMapRoleId(roleId);
    setMapLabel(label ?? "");
    setShowMap(true);
  }

  return (
    <>
      <div>
        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 24 }}>
          <div>
            <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
              Coğrafi Yetkiler
            </h1>
          </div>
          <button
            onClick={() => openMap(undefined, undefined, "Genel Sınır")}
            style={{
              padding:      "8px 16px",
              borderRadius: 8,
              border:       "none",
              background:   "#0f172a",
              color:        "#94a3b8",
              fontSize:     13,
              fontWeight:   500,
              cursor:       "pointer",
            }}
          >
            + Yeni Sınır Ekle
          </button>
        </div>

        {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}

        {!isLoading && (
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
            {permissions.length === 0 ? (
              <p style={{ padding: 24, color: "#94a3b8", fontSize: 13, textAlign: "center" }}>
                Henüz coğrafi yetki tanımlanmamış.
              </p>
            ) : (
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
                <thead>
                  <tr style={{ background: "#f8fafc" }}>
                    <th style={thStyle}>ID</th>
                    <th style={thStyle}>Kullanıcı ID</th>
                    <th style={thStyle}>Rol ID</th>
                    <th style={thStyle}>Durum</th>
                    <th style={thStyle}>İşlem</th>
                  </tr>
                </thead>
                <tbody>
                  {permissions.map((p) => (
                    <tr key={p.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                      <td style={tdStyle}>{p.id}</td>
                      <td style={tdStyle}>{p.userId ?? "—"}</td>
                      <td style={tdStyle}>{p.roleId ?? "—"}</td>
                      <td style={tdStyle}>
                        <span style={{
                          padding:      "2px 8px",
                          borderRadius: 20,
                          background:   p.isActive ? "#f0fdf4" : "#fef2f2",
                          color:        p.isActive ? "#16a34a" : "#dc2626",
                          fontSize:     11,
                          fontWeight:   500,
                        }}>
                          {p.isActive ? "Aktif" : "Pasif"}
                        </span>
                      </td>
                      <td style={tdStyle}>
                        <button
                          onClick={() => handleDelete(p.id)}
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
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        )}
      </div>

      {showMap && (
        <GeoPermissionMap
          userId={mapUserId}
          roleId={mapRoleId}
          label={mapLabel}
          onClose={() => setShowMap(false)}
          onSaved={() => { fetchPermissions(); setShowMap(false); }}
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