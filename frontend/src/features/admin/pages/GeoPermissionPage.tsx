import { useState, useEffect } from "react";
import { useAuth }             from "../../auth/context/AuthContext";
import { GeoPermissionMap }    from "../components/GeoPermissionMap";
import { ConfirmModal } from "../../../shared/components/ConfirmModal";

interface GeoPermissionDto {
  id:          number;
  name:        string;
  wktGeometry: string;
  isActive:    boolean;
}

export function GeoPermissionPage() {
  const [permissions, setPermissions] = useState<GeoPermissionDto[]>([]);
  const [isLoading,   setIsLoading]   = useState(true);
  const [showMap,     setShowMap]     = useState(false);
  const [editItem, setEditItem] = useState<GeoPermissionDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<GeoPermissionDto | null>(null);

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

  async function handleDeleteConfirmed() {
    if (!deleteTarget) return;
    try {
      await apiFetch(`/api/geo-permission/${deleteTarget.id}`, { method: "DELETE" });
      fetchPermissions();
    } catch {  }
    finally { setDeleteTarget(null); }
  }

  return (
    <>
      <div>
        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 24 }}>
          <div>
            <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
              Coğrafi Sınır Kütüphanesi
            </h1>
            <p style={{ fontSize: 14, color: "#64748b" }}>
              Rol ve kullanıcılara atanabilecek coğrafi sınırları yönetin.
            </p>
          </div>
          <button
            onClick={() => setShowMap(true)}
            style={{
              padding: "8px 16px", borderRadius: 8,
              border: "none", background: "#0f172a",
              color: "#94a3b8", fontSize: 13,
              fontWeight: 500, cursor: "pointer",
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
                Henüz coğrafi sınır tanımlanmamış.
              </p>
            ) : (
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
                <thead>
                  <tr style={{ background: "#f8fafc" }}>
                    <th style={thStyle}>ID</th>
                    <th style={thStyle}>İsim</th>
                    <th style={thStyle}>Durum</th>
                    <th style={thStyle}>İşlem</th>
                  </tr>
                </thead>
                <tbody>
                  {permissions.map((p) => (
                    <tr key={p.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                      <td style={tdStyle}>{p.id}</td>
                      <td style={tdStyle}>{p.name}</td>
                      <td style={tdStyle}>
                        <span style={{
                          padding: "2px 8px", borderRadius: 20,
                          background: p.isActive ? "#f0fdf4" : "#fef2f2",
                          color:      p.isActive ? "#16a34a" : "#dc2626",
                          fontSize: 11, fontWeight: 500,
                        }}>
                          {p.isActive ? "Aktif" : "Pasif"}
                        </span>
                      </td>
                      <td style={tdStyle}>
                        <div style={{ display: "flex", gap: 6 }}>
                          <button
                            onClick={() => setEditItem(p)}
                            style={{
                              padding: "4px 10px", borderRadius: 6,
                              border: "1px solid rgba(59,130,246,.3)",
                              background: "rgba(59,130,246,.05)",
                              color: "#3b82f6", fontSize: 12, cursor: "pointer",
                            }}
                          >
                            Değiştir
                          </button>
                          <button
                            onClick={() => setDeleteTarget(p)}
                            style={{
                              padding: "4px 10px", borderRadius: 6,
                              border: "1px solid rgba(239,68,68,.3)",
                              background: "rgba(239,68,68,.05)",
                              color: "#ef4444", fontSize: 12, cursor: "pointer",
                            }}
                          >
                            Sil
                          </button>
                        </div>
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
          onClose={() => setShowMap(false)}
          onSaved={() => { fetchPermissions(); setShowMap(false); }}
        />
      )}
      {editItem && (
        <GeoPermissionMap
          editId={editItem.id}
          existingWkt={editItem.wktGeometry}
          existingName={editItem.name}
          onClose={() => setEditItem(null)}
          onSaved={() => { fetchPermissions(); setEditItem(null); }}
        />
      )}

      {deleteTarget && (
        <ConfirmModal
          message={`"${deleteTarget.name}" sınırı silinecek. Bu işlem geri alınamaz.`}
          onConfirm={handleDeleteConfirmed}
          onCancel={() => setDeleteTarget(null)}
        />
      )}
    </>
  );
}

const thStyle: React.CSSProperties = {
  padding: "10px 16px", textAlign: "left",
  fontSize: 11, fontWeight: 600,
  color: "#64748b", letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};