import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getTeams } from "../api/teamService";
import { assignTeamToUser } from "../api/userService";

interface TeamDto {
  id:   number;
  name: string;
}

interface TeamAssignModalProps {
  userId:        number;
  userEmail:     string;
  currentTeamId: number | null;
  onClose:       () => void;
  onUpdated:     () => void;
}

export function TeamAssignModal({
  userId,
  userEmail,
  currentTeamId,
  onClose,
  onUpdated,
}: TeamAssignModalProps) {
  const [teams,      setTeams]      = useState<TeamDto[]>([]);
  const [selectedId, setSelectedId] = useState<number | null>(currentTeamId);
  const [isLoading,  setIsLoading]  = useState(true);
  const [isSaving,   setIsSaving]   = useState(false);
  const [error,      setError]      = useState<string | null>(null);

  const { apiFetch } = useAuth();

  useEffect(() => {
    async function fetchTeams() {
      setIsLoading(true);
      try {
        const res = await getTeams(apiFetch);
        if (!res.ok) { setError("Takımlar yüklenemedi."); return; }
        setTeams(await res.json());
      } catch {
        setError("Sunucuya bağlanılamadı.");
      } finally {
        setIsLoading(false);
      }
    }
    fetchTeams();
  }, []);

  async function handleSave() {
    setIsSaving(true);
    setError(null);
    try {
      const res = await assignTeamToUser(apiFetch, userId, selectedId);
      if (!res.ok) { setError("Kaydedilemedi."); return; }
      onUpdated();
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
        position: "fixed", inset: 0, zIndex: 3000,
        background: "rgba(0,0,0,0.4)",
        display: "flex", alignItems: "center", justifyContent: "center",
      }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "#ffffff", borderRadius: 16,
          padding: "28px 32px", width: 420,
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 4px" }}>
          Takım Ata
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 20px" }}>
          Kullanıcı: <strong>{userEmail}</strong>
        </p>

        {isLoading ? (
          <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 8, marginBottom: 20 }}>
            <label style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 10px", borderRadius: 8, cursor: "pointer" }}>
              <input
                type="radio"
                name="team"
                checked={selectedId === null}
                onChange={() => setSelectedId(null)}
              />
              <span style={{ fontSize: 13, color: "#64748b" }}>Takımsız</span>
            </label>
            {teams.map((t) => (
              <label
                key={t.id}
                style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 10px", borderRadius: 8, cursor: "pointer" }}
              >
                <input
                  type="radio"
                  name="team"
                  checked={selectedId === t.id}
                  onChange={() => setSelectedId(t.id)}
                />
                <span style={{ fontSize: 13, color: "#0f172a" }}>{t.name}</span>
              </label>
            ))}
          </div>
        )}

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            onClick={onClose}
            disabled={isSaving}
            style={{
              flex: 1, padding: "9px 0", borderRadius: 8,
              border: "1px solid #e2e8f0", background: "#f8fafc",
              color: "#64748b", fontSize: 13, cursor: "pointer",
            }}
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={isSaving}
            style={{
              flex: 2, padding: "9px 0", borderRadius: 8, border: "none",
              background: isSaving ? "#94a3b8" : "#0f172a",
              color: "#ffffff", fontSize: 13, fontWeight: 600,
              cursor: isSaving ? "not-allowed" : "pointer",
            }}
          >
            {isSaving ? "Kaydediliyor..." : "Kaydet"}
          </button>
        </div>
      </div>
    </div>
  );
}