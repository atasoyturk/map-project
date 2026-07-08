import { useState, useEffect } from "react";
import { useAuth }             from "../../auth/context/AuthContext";
import { GeoPermissionMap }    from "./GeoPermissionMap";

interface GeoPermissionDto {
  id:          number;
  name:        string;
  wktGeometry: string;
  isActive:    boolean;
}

interface UserGeoPermissionModalProps {
  userId:    number;
  userEmail: string;
  onClose:   () => void;
}

type Tab = "list" | "new";

export function UserGeoPermissionModal({
  userId,
  userEmail,
  onClose,
}: UserGeoPermissionModalProps) {
  const [tab,                 setTab]                 = useState<Tab>("list");
  const [allPermissions,      setAllPermissions]      = useState<GeoPermissionDto[]>([]);
  const [assignedPermissions, setAssignedPermissions] = useState<Set<number>>(new Set());
  const [isLoading,           setIsLoading]           = useState(true);
  const [isSaving,            setIsSaving]            = useState(false);
  const [showDrawMap,         setShowDrawMap]         = useState(false);

  const { apiFetch } = useAuth();

  async function fetchData() {
    setIsLoading(true);
    try {
      const [allRes, assignedRes] = await Promise.all([
        apiFetch("/api/geo-permission"),
        apiFetch(`/api/geo-permission/user/${userId}`),
      ]);

      if (!allRes.ok || !assignedRes.ok) return;

      const all:      GeoPermissionDto[] = await allRes.json();
      const assigned: GeoPermissionDto[] = await assignedRes.json();

      setAllPermissions(all);
      setAssignedPermissions(new Set(assigned.map((p) => p.id)));
    } catch {  }
    finally { setIsLoading(false); }
  }

  useEffect(() => { fetchData(); }, [userId]);

  async function handleToggle(permissionId: number) {
    const isAssigned = assignedPermissions.has(permissionId);
    setIsSaving(true);
    try {
      if (isAssigned) {
        await apiFetch(`/api/geo-permission/user/${userId}/${permissionId}`, {
          method: "DELETE",
        });
        setAssignedPermissions((prev) => {
          const next = new Set(prev);
          next.delete(permissionId);
          return next;
        });
      } else {
        await apiFetch(`/api/geo-permission/user/${userId}/${permissionId}`, {
          method: "POST",
        });
        setAssignedPermissions((prev) => new Set(prev).add(permissionId));
      }
    } catch {  }
    finally { setIsSaving(false); }
  }

  // when a new boundary saved, refresh library and assign to the user
  async function handleNewSaved() {
    // Kütüphaneyi yenile
    const allRes = await apiFetch("/api/geo-permission");
    if (!allRes.ok) return;
    const all: GeoPermissionDto[] = await allRes.json();
    setAllPermissions(all);

    const latest = all[all.length - 1];
    if (latest) {
      await apiFetch(`/api/geo-permission/user/${userId}/${latest.id}`, {
        method: "POST",
      });
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

          {/* Section*/}
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

          {/* choose from lib */}
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
            )
          )}

          {/* Add new boundary */}
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

      {/* map */}
      {showDrawMap && (
        <GeoPermissionMap
          onClose={() => setShowDrawMap(false)}
          onSaved={handleNewSaved}
        />
      )}
    </>
  );
}