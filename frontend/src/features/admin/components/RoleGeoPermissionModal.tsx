import { useState, useEffect } from "react";
import { useAuth }             from "../../auth/context/AuthContext";

interface GeoPermissionDto {
  id:          number;
  name:        string;
  wktGeometry: string;
  isActive:    boolean;
}

interface RoleGeoPermissionModalProps {
  roleId:   number;
  roleName: string;
  onClose:  () => void;
}

export function RoleGeoPermissionModal({
  roleId,
  roleName,
  onClose,
}: RoleGeoPermissionModalProps) {
  const [allPermissions,      setAllPermissions]      = useState<GeoPermissionDto[]>([]);
  const [assignedPermissions, setAssignedPermissions] = useState<Set<number>>(new Set());
  const [isLoading,           setIsLoading]           = useState(true);
  const [isSaving,            setIsSaving]            = useState(false);

  const { apiFetch } = useAuth();

  async function fetchData() {
    setIsLoading(true);
    try {
      const [allRes, assignedRes] = await Promise.all([
        apiFetch("/api/geo-permission"),
        apiFetch(`/api/geo-permission/role/${roleId}`),
      ]);

      if (!allRes.ok || !assignedRes.ok) return;

      const all:      GeoPermissionDto[] = await allRes.json();
      const assigned: GeoPermissionDto[] = await assignedRes.json();

      setAllPermissions(all);
      setAssignedPermissions(new Set(assigned.map((p) => p.id)));
    } catch { /* sessiz */ }
    finally { setIsLoading(false); }
  }

  useEffect(() => { fetchData(); }, [roleId]);

  async function handleToggle(permissionId: number) {
    const isAssigned = assignedPermissions.has(permissionId);
    setIsSaving(true);
    try {
      if (isAssigned) {
        await apiFetch(`/api/geo-permission/role/${roleId}/${permissionId}`, {
          method: "DELETE",
        });
        setAssignedPermissions((prev) => {
          const next = new Set(prev);
          next.delete(permissionId);
          return next;
        });
      } else {
        await apiFetch(`/api/geo-permission/role/${roleId}/${permissionId}`, {
          method: "POST",
        });
        setAssignedPermissions((prev) => new Set(prev).add(permissionId));
      }
    } catch { /* sessiz */ }
    finally { setIsSaving(false); }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 3000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 500,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
          maxHeight: "80vh", overflowY: "auto",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 4px" }}>
          Coğrafi Sınır Ata
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 20px" }}>
          Rol: <strong>{roleName}</strong>
        </p>

        {isLoading ? (
          <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
        ) : allPermissions.length === 0 ? (
          <p style={{ color: "#94a3b8", fontSize: 13 }}>
            Henüz coğrafi sınır tanımlanmamış. Önce Coğrafi Yetkiler sayfasından sınır ekleyin.
          </p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {allPermissions.map((p) => {
              const isAssigned = assignedPermissions.has(p.id);
              return (
                <div
                  key={p.id}
                  style={{
                    display: "flex", alignItems: "center", gap: 12,
                    padding: "12px 14px", borderRadius: 10,
                    border: "1px solid #e2e8f0",
                    background: isAssigned ? "#eff6ff" : "#ffffff",
                  }}
                >
                  <input
                    type="checkbox"
                    checked={isAssigned}
                    disabled={isSaving}
                    onChange={() => handleToggle(p.id)}
                    style={{ cursor: isSaving ? "not-allowed" : "pointer" }}
                  />
                  <div style={{ flex: 1 }}>
                    <span style={{ fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                      {p.name}
                    </span>
                    {isAssigned && (
                      <span style={{
                        marginLeft: 8, padding: "1px 8px",
                        borderRadius: 20, background: "#eff6ff",
                        color: "#3b82f6", fontSize: 10, fontWeight: 600,
                      }}>
                        Atanmış
                      </span>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            width: "100%", marginTop: 20, padding: "9px 0",
            borderRadius: 8, border: "1px solid #e2e8f0",
            background: "#f8fafc", color: "#64748b",
            fontSize: 13, cursor: "pointer",
          }}
        >
          Kapat
        </button>
      </div>
    </div>
  );
}