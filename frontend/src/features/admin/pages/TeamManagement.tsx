import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";

interface TeamDto {
  id:   number;
  name: string;
}

export function TeamManagement() {
  const [teams,     setTeams]     = useState<TeamDto[]>([]);
  const [newTeam,   setNewTeam]   = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving,  setIsSaving]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);

  const { apiFetch } = useAuth();

  async function fetchTeams() {
    setIsLoading(true);
    try {
      const res = await apiFetch("/api/admin/teams");
      if (!res.ok) { setError("Takımlar yüklenemedi."); return; }
      setTeams(await res.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchTeams(); }, []);

  async function handleCreate() {
    if (!newTeam.trim()) return;
    setIsSaving(true);
    try {
      const res = await apiFetch("/api/admin/teams", {
        method: "POST",
        body:   JSON.stringify({ name: newTeam.trim() }),
      });
      if (!res.ok) return;
      setNewTeam("");
      fetchTeams();
    } catch { }
    finally { setIsSaving(false); }
  }

  async function handleDelete(id: number) {
    if (!confirm("Bu takımı silmek istediğinize emin misiniz?")) return;
    try {
      const res = await apiFetch(`/api/admin/teams/${id}`, { method: "DELETE" });
      if (!res.ok) { setError("Takım silinemedi."); return; }
      fetchTeams();
    } catch { }
  }

  return (
    <div>
      <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
        Takım Yönetimi
      </h1>
      <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
        Sistem takımlarını yönetin.
      </p>

      <div style={{
        background: "#ffffff", borderRadius: 12,
        border: "1px solid #e2e8f0", padding: 16,
        marginBottom: 24, display: "flex", gap: 8,
      }}>
        <input
          type="text"
          placeholder="Yeni takım adı..."
          value={newTeam}
          onChange={(e) => setNewTeam(e.target.value)}
          style={{
            flex: 1, padding: "8px 12px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13,
            color: "#0f172a", outline: "none",
          }}
        />
        <button
          onClick={handleCreate}
          disabled={isSaving || !newTeam.trim()}
          style={{
            padding:      "8px 16px",
            borderRadius: 8,
            border:       "none",
            background:   "#0f172a",
            color:        "#94a3b8",
            fontSize:     13,
            fontWeight:   500,
            cursor:       isSaving ? "not-allowed" : "pointer",
            opacity:      isSaving ? 0.6 : 1,
          }}
        >
          Ekle
        </button>
      </div>

      {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
      {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

      {!isLoading && !error && (
        <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
            <thead>
              <tr style={{ background: "#f8fafc" }}>
                <th style={thStyle}>ID</th>
                <th style={thStyle}>Takım Adı</th>
                <th style={thStyle}>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {teams.map((team) => (
                <tr key={team.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                  <td style={tdStyle}>{team.id}</td>
                  <td style={tdStyle}>{team.name}</td>
                  <td style={tdStyle}>
                    <button
                      onClick={() => handleDelete(team.id)}
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
        </div>
      )}
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding:    "10px 16px",
  textAlign:  "left",
  fontSize:   11,
  fontWeight: 600,
  color:      "#64748b",
  letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};