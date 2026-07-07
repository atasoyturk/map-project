import { useState, useEffect } from "react";
import { useAuth } from "../../context/AuthContext";

interface EffectivePermission {
  name:        string;
  description: string;
  isGranted:   boolean;
  origin:      string;      // "Role" | "User" | "None"
  roleName:    string | null;
}

interface PermissionMatrixProps {
  userId:   number;
  onClose:  () => void;
}

export function PermissionMatrix({ userId, onClose }: PermissionMatrixProps) {
  const [permissions, setPermissions] = useState<EffectivePermission[]>([]);
  const [isLoading,   setIsLoading]   = useState(true);
  const [isSaving,    setIsSaving]    = useState(false);

  const { apiFetch } = useAuth();

  async function fetchPermissions() {
    setIsLoading(true);
    try {
      const res = await apiFetch(`/api/admin/users/${userId}/permissions`);
      if (!res.ok) return;
      setPermissions(await res.json());
    } catch {  }
    finally { setIsLoading(false); }
  }

  useEffect(() => { fetchPermissions(); }, [userId]);

  async function handleToggle(permission: EffectivePermission) {
    if (permission.origin === "Role") return;

    const permissionId = getPermissionId(permission.name);
    if (!permissionId) return;

    setIsSaving(true);
    try {
        if (permission.isGranted) {
        await apiFetch(`/api/admin/users/${userId}/permissions/${permissionId}`, {
            method: "DELETE",
        });
        } else {
        await apiFetch(`/api/admin/users/${userId}/permissions`, {
            method: "POST",
            body:   JSON.stringify({ permissionId }),
        });
        }
        fetchPermissions();
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
          padding: "28px 32px", width: 480,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
          maxHeight: "80vh", overflowY: "auto",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 6px" }}>
          Yetkiler
        </h2>
      
        {isLoading ? (
          <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            {permissions.map((p) => (
              <div
                key={p.name}
                style={{
                  display:      "flex",
                  alignItems:   "flex-start",
                  gap:          12,
                  padding:      "12px 14px",
                  borderRadius: 10,
                  border:       "1px solid #e2e8f0",
                  background:   p.origin === "Role" ? "#f8fafc" : "#ffffff",
                }}
              >
                <input
                  type="checkbox"
                  checked={p.isGranted}
                  disabled={p.origin === "Role" || isSaving}
                  onChange={() => handleToggle(p)}
                  style={{ marginTop: 2, cursor: p.origin === "Role" ? "not-allowed" : "pointer" }}
                />
                <div style={{ flex: 1 }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 2 }}>
                    <span style={{ fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                      {p.name}
                    </span>
                    {p.origin === "Role" && p.roleName && (
                      <span style={{
                        padding:      "1px 8px",
                        borderRadius: 20,
                        background:   "#eff6ff",
                        color:        "#3b82f6",
                        fontSize:     10,
                        fontWeight:   600,
                      }}>
                        {p.roleName} Rolünden
                      </span>
                    )}
                    {p.origin === "User" && (
                      <span style={{
                        padding:      "1px 8px",
                        borderRadius: 20,
                        background:   "#f0fdf4",
                        color:        "#16a34a",
                        fontSize:     10,
                        fontWeight:   600,
                      }}>
                        Kullanıcıya Özel
                      </span>
                    )}
                  </div>
                  <span style={{ fontSize: 11, color: "#94a3b8" }}>{p.description}</span>
                </div>
              </div>
            ))}
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            width:        "100%",
            marginTop:    20,
            padding:      "9px 0",
            borderRadius: 8,
            border:       "1px solid #e2e8f0",
            background:   "#f8fafc",
            color:        "#64748b",
            fontSize:     13,
            cursor:       "pointer",
          }}
        >
          Kapat
        </button>
      </div>
    </div>
  );
}

// id comes from backend
function getPermissionId(name: string): number {
  const map: Record<string, number> = {
    "point_create":   1,
    "line_create":    2,
    "polygon_create": 3,
    "admin_access":   4,
  };
  return map[name] ?? 0;
}