import { useState, useEffect } from "react";
import { useAuth }             from "../../auth/context/AuthContext";
import { GeoPermissionMap }    from "./GeoPermissionMap";
import { getRoles }            from "../api/roleService";
import {
  getAllGeoPermissions,
  getGeoPermissionsByUser,
  getGeoPermissionsByRole,
  assignGeoPermissionToUser,
  removeGeoPermissionFromUser,
} from "../api/geoPermissionService";

interface GeoPermissionDto {
  id:          number;
  name:        string;
  wktGeometry: string;
  isActive:    boolean;
}

interface UserGeoPermissionModalProps {
  userId:    number;
  userEmail: string;
  userRoles: string[];
  onClose:   () => void;
}

type Tab = "list" | "new";

export function UserGeoPermissionModal({
  userId,
  userEmail,
  userRoles,
  onClose,
}: UserGeoPermissionModalProps) {
  const [tab,                 setTab]                 = useState<Tab>("list");
  const [allPermissions,      setAllPermissions]      = useState<GeoPermissionDto[]>([]);
  const [assignedPermissions, setAssignedPermissions] = useState<Set<number>>(new Set());
  const [isLoading,           setIsLoading]           = useState(true);
  const [isSaving,            setIsSaving]            = useState(false);
  const [showDrawMap,         setShowDrawMap]         = useState(false);
  const [roleAssignedPermissions, setRoleAssignedPermissions] = useState<Set<number>>(new Set());

  const { apiFetch } = useAuth();

  async function fetchData() {
    setIsLoading(true);
    try {
      const rolesRes = await getRoles(apiFetch);
      const allRoles: { id: number; name: string }[] = rolesRes.ok ? await rolesRes.json() : [];
      const userRoleIds = allRoles
        .filter(r => userRoles.includes(r.name))
        .map(r => r.id);

      const [allRes, assignedRes, ...roleResponses] = await Promise.all([
        getAllGeoPermissions(apiFetch),
        getGeoPermissionsByUser(apiFetch, userId),
        ...userRoleIds.map(roleId => getGeoPermissionsByRole(apiFetch, roleId)),
      ]);

      if (!allRes.ok || !assignedRes.ok) return;

      const all:      GeoPermissionDto[] = await allRes.json();
      const assigned: GeoPermissionDto[] = await assignedRes.json();

      const roleAssigned: GeoPermissionDto[] = [];
      for (const res of roleResponses) {
        if (res.ok) {
          const data: GeoPermissionDto[] = await res.json();
          roleAssigned.push(...data);
        }
      }

      setAllPermissions(all);
      setAssignedPermissions(new Set(assigned.map(p => p.id)));
      setRoleAssignedPermissions(new Set(roleAssigned.map(p => p.id)));
    } catch { /* sessiz */ }
    finally { setIsLoading(false); }
  }

  useEffect(() => { fetchData(); }, [userId]);

  async function handleToggle(permissionId: number) {
    const isAssigned = assignedPermissions.has(permissionId);
    setIsSaving(true);
    try {
      if (isAssigned) {
        await removeGeoPermissionFromUser(apiFetch, userId, permissionId);
        setAssignedPermissions((prev) => {
          const next = new Set(prev);
          next.delete(permissionId);
          return next;
        });
      } else {
        await assignGeoPermissionToUser(apiFetch, userId, permissionId);
        setAssignedPermissions((prev) => new Set(prev).add(permissionId));
      }
    } catch {  }
    finally { setIsSaving(false); }
  }

  async function handleNewSaved() {
    const allRes = await getAllGeoPermissions(apiFetch);
    if (!allRes.ok) return;
    const all: GeoPermissionDto[] = await allRes.json();
    setAllPermissions(all);

    const latest = all[all.length - 1];
    if (latest) {
      await assignGeoPermissionToUser(apiFetch, userId, latest.id);
      setAssignedPermissions((prev) => new Set(prev).add(latest.id));
    }

    setShowDrawMap(false);
    setTab("list");
  }

  return (
    <>
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
            padding: "28px 32px", width: 520,
            boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
            maxHeight: "80vh", overflowY: "auto",
          }}
        >
          <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 4px" }}>
            Özel Coğrafi Yetki
          </h2>
          <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 20px" }}>
            Kullanıcı: <strong>{userEmail}</strong>
          </p>

          <div style={{ display: "flex", gap: 4, marginBottom: 20, borderBottom: "1px solid #e2e8f0" }}>
            {([["list", "Kütüphaneden Seç"], ["new", "Yeni Sınır Ekle"]] as [Tab, string][]).map(([key, label]) => (
              <button
                key={key}
                onClick={() => setTab(key)}
                style={{
                  padding: "8px 16px", borderRadius: "8px 8px 0 0",
                  border: "none", fontSize: 13, cursor: "pointer",
                  background:  tab === key ? "#0f172a" : "transparent",
                  color:       tab === key ? "#94a3b8" : "#64748b",
                  fontWeight:  tab === key ? 600 : 400,
                  borderBottom: tab === key ? "2px solid #3b82f6" : "2px solid transparent",
                }}
              >
                {label}
              </button>
            ))}
          </div>

          {tab === "list" && (
            isLoading ? (
              <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
            ) : allPermissions.length === 0 ? (
              <p style={{ color: "#94a3b8", fontSize: 13 }}>
                Henüz coğrafi sınır tanımlanmamış. "Yeni Sınır Ekle" sekmesinden oluşturun.
              </p>
            ) : (
              <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
                {allPermissions.map((p) => {
                  const isUserAssigned = assignedPermissions.has(p.id);
                  const isRoleAssigned = roleAssignedPermissions.has(p.id);
                  const isAssigned     = isUserAssigned || isRoleAssigned;
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
                        disabled={isSaving || isRoleAssigned}
                        onChange={() => !isRoleAssigned && handleToggle(p.id)}
                        style={{ cursor: isSaving || isRoleAssigned ? "not-allowed" : "pointer" }}
                      />
                      <div style={{ flex: 1 }}>
                        <span style={{ fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                          {p.name}
                        </span>
                        {isRoleAssigned && (
                          <span style={{
                            marginLeft: 8, padding: "1px 8px",
                            borderRadius: 20, background: "#eff6ff",
                            color: "#3b82f6", fontSize: 10, fontWeight: 600,
                          }}>
                            Rolden Geliyor
                          </span>
                        )}
                        {isUserAssigned && !isRoleAssigned && (
                          <span style={{
                            marginLeft: 8, padding: "1px 8px",
                            borderRadius: 20, background: "#f0fdf4",
                            color: "#16a34a", fontSize: 10, fontWeight: 600,
                          }}>
                            Kullanıcıya Özel
                          </span>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )
          )}

          {tab === "new" && (
            <div style={{ textAlign: "center", padding: "20px 0" }}>
              <p style={{ fontSize: 13, color: "#64748b", marginBottom: 16 }}>
                Haritada yeni bir coğrafi sınır çizerek kütüphaneye ekleyin ve bu kullanıcıya atayın.
              </p>
              <button
                onClick={() => setShowDrawMap(true)}
                style={{
                  padding: "10px 24px", borderRadius: 8,
                  border: "none", background: "#0f172a",
                  color: "#94a3b8", fontSize: 13,
                  fontWeight: 500, cursor: "pointer",
                }}
              >
                Haritayı Aç
              </button>
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

      {showDrawMap && (
        <GeoPermissionMap
          onClose={() => setShowDrawMap(false)}
          onSaved={handleNewSaved}
        />
      )}
    </>
  );
}