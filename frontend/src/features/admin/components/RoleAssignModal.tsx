import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";

interface RoleDto {
  id:   number;
  name: string;
}

interface RoleAssignModalProps {
  userId:       number;
  currentRoles: string[];
  onClose:      () => void;
  onUpdated:    () => void;
}

export function RoleAssignModal({ userId, currentRoles, onClose, onUpdated }: RoleAssignModalProps) {
  const [allRoles,  setAllRoles]  = useState<RoleDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving,  setIsSaving]  = useState(false);

  const { apiFetch, roles } = useAuth();

  useEffect(() => {
    async function fetchRoles() {
      setIsLoading(true);
      try {
        const res = await apiFetch("/api/admin/roles");
        if (!res.ok) return;
        setAllRoles(await res.json());
      } catch {  }
      finally { setIsLoading(false); }
    }
    fetchRoles();
  }, []);

  async function handleToggle(role: RoleDto) {
    const hasRole = currentRoles.includes(role.name);
    setIsSaving(true);
    try {
      if (hasRole) {
        await apiFetch(`/api/admin/users/${userId}/roles/${role.id}`, {
          method: "DELETE",
        });
      } else {
        await apiFetch(`/api/admin/users/${userId}/roles`, {
          method: "POST",
          body:   JSON.stringify({ roleId: role.id }),
        });
      }
      onUpdated();
    } catch {  }
    finally { setIsSaving(false); }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position:  "fixed", inset: 0, zIndex: 3000,
        background: "rgba(0,0,0,0.4)",
        display:   "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background:   "#ffffff", borderRadius: 16,
          padding:      "28px 32px", width: 400,
          boxShadow:    "0 20px 60px rgba(0,0,0,0.25)",
          maxHeight:    "80vh", overflowY: "auto",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 6px" }}>
          Rol Yönetimi
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 20px" }}>
          Kullanıcının rollerini düzenleyin.
        </p>

        {isLoading ? (
          <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {allRoles
              .filter(role => role.name !== "Admin" || roles.includes("Admin"))
              .map((role) => {
              const hasRole = currentRoles.includes(role.name);
              return (
                <div
                  key={role.id}
                  style={{
                    display:      "flex",
                    alignItems:   "center",
                    gap:          12,
                    padding:      "12px 14px",
                    borderRadius: 10,
                    border:       "1px solid #e2e8f0",
                    background:   hasRole ? "#eff6ff" : "#ffffff",
                  }}
                >
                  <input
                    type="checkbox"
                    checked={hasRole}
                    disabled={isSaving}
                    onChange={() => handleToggle(role)}
                    style={{ cursor: isSaving ? "not-allowed" : "pointer" }}
                  />
                  <span style={{ fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                    {role.name}
                  </span>
                  {hasRole && (
                    <span style={{
                      marginLeft:   "auto",
                      padding:      "1px 8px",
                      borderRadius: 20,
                      background:   "#eff6ff",
                      color:        "#3b82f6",
                      fontSize:     10,
                      fontWeight:   600,
                    }}>
                      Atanmış
                    </span>
                  )}
                </div>
              );
            })}
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