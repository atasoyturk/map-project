import { useState } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { assignTeamToUsers } from "../api/userService";

interface EligibleUser {
  id:       number;
  email:    string;
  teamName: string | null;
}

interface AddTeamMembersModalProps {
  teamId:   number;
  teamName: string;
  users:    EligibleUser[];
  onClose:  () => void;
  onAdded:  () => void;
}

export function AddTeamMembersModal({ teamId, teamName, users, onClose, onAdded }: AddTeamMembersModalProps) {
  const { apiFetch } = useAuth();

  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);

  function toggle(userId: number) {
    setSelected((prev) => {
      const next = new Set(prev);
      next.has(userId) ? next.delete(userId) : next.add(userId);
      return next;
    });
  }

  async function handleAdd() {
    if (selected.size === 0 || isSaving) return;
    setIsSaving(true);
    setError(null);
    try {
      const res = await assignTeamToUsers(apiFetch, Array.from(selected), teamId);
      if (!res.ok) { setError("Ekleme başarısız oldu."); return; }
      onAdded();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, zIndex: 2000,
        background: "rgba(0,0,0,0.5)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 400, maxHeight: "70vh",
          display: "flex", flexDirection: "column",
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 20px" }}>
          "{teamName}" Takımına Eleman Ekle
        </h2>

        {users.length === 0 ? (
          <p style={{ fontSize: 13, color: "#94a3b8", marginBottom: 20 }}>
            Eklenebilecek başka kullanıcı yok.
          </p>
        ) : (
          <div style={{ flex: 1, overflowY: "auto", marginBottom: 20, display: "flex", flexDirection: "column", gap: 6 }}>
            {users.map((user) => (
              <label
                key={user.id}
                style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 13, padding: "6px 8px", borderRadius: 6 }}
              >
                <input
                  type="checkbox"
                  checked={selected.has(user.id)}
                  onChange={() => toggle(user.id)}
                />
                <span style={{ flex: 1, color: "#374151" }}>{user.email}</span>
                <span style={{ fontSize: 11, color: "#94a3b8" }}>
                  {user.teamName ?? "Takımsız"}
                </span>
              </label>
            ))}
          </div>
        )}

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 16px" }}>{error}</p>}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onClose}
            disabled={isSaving}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleAdd}
            disabled={selected.size === 0 || isSaving}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
              background: selected.size === 0 || isSaving ? "#94a3b8" : "#0f172a",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: selected.size === 0 || isSaving ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Ekleniyor..." : `Ekle (${selected.size})`}
          </button>
        </div>
      </div>
    </div>
  );
}