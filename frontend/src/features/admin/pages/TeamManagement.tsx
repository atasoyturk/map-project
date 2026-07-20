import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getTeams, createTeam, deleteTeam } from "../api/teamService";
import { getUsers, assignTeamToUsers } from "../api/userService";

interface TeamDto {
  id:   number;
  name: string;
}

interface UserDto {
  id:       number;
  email:    string;
  isActive: boolean;
  roles:    string[];
  teamId:   number | null;
  teamName: string | null;
}

type TargetTeam = number | "unassigned" | "";

export function TeamManagement() {
  const [teams,     setTeams]     = useState<TeamDto[]>([]);
  const [newTeam,   setNewTeam]   = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving,  setIsSaving]  = useState(false);
  const [error,     setError]     = useState<string | null>(null);

  const [users,           setUsers]           = useState<UserDto[]>([]);
  const [selectedByTeam,  setSelectedByTeam]  = useState<Record<number, Set<number>>>({});
  const [targetByTeam,    setTargetByTeam]    = useState<Record<number, TargetTeam>>({});
  const [isMoving,        setIsMoving]        = useState<number | null>(null);

  const { apiFetch } = useAuth();

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

  async function fetchUsers() {
    try {
      const res = await getUsers(apiFetch);
      if (!res.ok) return;
      setUsers(await res.json());
    } catch { }
  }

  function toggleMember(teamId: number, userId: number) {
    setSelectedByTeam((prev) => {
      const current = new Set(prev[teamId] ?? []);
      current.has(userId) ? current.delete(userId) : current.add(userId);
      return { ...prev, [teamId]: current };
    });
  }

  async function handleMove(teamId: number) {
    const selected = selectedByTeam[teamId];
    const target   = targetByTeam[teamId];
    if (!selected || selected.size === 0 || target === "") return;

    setIsMoving(teamId);
    try {
      const teamIdToAssign = target === "unassigned" ? null : target;
      const res = await assignTeamToUsers(apiFetch, Array.from(selected), teamIdToAssign);
      if (!res.ok) { setError("Taşıma işlemi başarısız oldu."); return; }
      setSelectedByTeam((prev) => ({ ...prev, [teamId]: new Set() }));
      setTargetByTeam((prev) => ({ ...prev, [teamId]: "" }));
      fetchTeams();
      fetchUsers();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsMoving(null);
    }
  }

  useEffect(() => { fetchTeams(); fetchUsers(); }, []);

  async function handleCreate() {
    if (!newTeam.trim()) return;
    setIsSaving(true);
    try {
      const res = await createTeam(apiFetch, newTeam.trim());
      if (!res.ok) return;
      setNewTeam("");
      fetchTeams();
    } catch { }
    finally { setIsSaving(false); }
  }

  async function handleDelete(id: number) {
    if (!confirm("Bu takımı silmek istediğinize emin misiniz?")) return;
    try {
      const res = await deleteTeam(apiFetch, id);
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

      {!isLoading && !error && (
        <div style={{ marginTop: 24 }}>
          <h2 style={{ fontSize: 16, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
            Takım Üyeleri
          </h2>

          <div style={{
            display:               "grid",
            gridTemplateColumns:   "repeat(auto-fill, minmax(300px, 1fr))",
            gap:                   16,
          }}>
            {teams.map((team) => {
              const members  = users.filter((u) => u.teamId === team.id);
              const selected = selectedByTeam[team.id] ?? new Set<number>();
              const target   = targetByTeam[team.id] ?? "";

              return (
                <div
                  key={team.id}
                  style={{
                    background:   "#ffffff",
                    borderRadius: 12,
                    border:       "1px solid #e2e8f0",
                    padding:      16,
                  }}
                >
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
                    <h3 style={{ fontSize: 14, fontWeight: 600, color: "#0f172a", margin: 0 }}>
                      {team.name}
                    </h3>
                    <span style={{ fontSize: 12, color: "#94a3b8" }}>
                      {members.length} kişi
                    </span>
                  </div>

                  {members.length === 0 ? (
                    <p style={{ fontSize: 12, color: "#94a3b8" }}>
                      Bu takımda kimse yok.
                    </p>
                  ) : (
                    <div style={{ display: "flex", flexDirection: "column", gap: 6, marginBottom: 12 }}>
                      {members.map((user) => (
                        <label
                          key={user.id}
                          style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 12 }}
                        >
                          <input
                            type="checkbox"
                            checked={selected.has(user.id)}
                            onChange={() => toggleMember(team.id, user.id)}
                          />
                          <span style={{ color: "#374151", flex: 1 }}>
                            {user.email}
                          </span>
                          <div style={{ display: "flex", gap: 4, flexWrap: "wrap" }}>
                            {user.roles.map((r) => (
                              <span
                                key={r}
                                style={{
                                  padding:      "1px 6px",
                                  borderRadius: 20,
                                  background:   "#eff6ff",
                                  color:        "#3b82f6",
                                  fontSize:     10,
                                  fontWeight:   500,
                                }}
                              >
                                {r}
                              </span>
                            ))}
                          </div>
                        </label>
                      ))}
                    </div>
                  )}

                  {members.length > 0 && (
                    <div style={{ display: "flex", gap: 6, borderTop: "1px solid #f1f5f9", paddingTop: 12 }}>
                      <select
                        value={target}
                        onChange={(e) => {
                          const v = e.target.value;
                          setTargetByTeam((prev) => ({
                            ...prev,
                            [team.id]: v === "" ? "" : v === "unassigned" ? "unassigned" : Number(v),
                          }));
                        }}
                        style={{
                          flex:         1,
                          padding:      "6px 8px",
                          borderRadius: 6,
                          border:       "1px solid #e2e8f0",
                          fontSize:     12,
                          color:        "#0f172a",
                        }}
                      >
                        <option value="">Takım seç...</option>
                        {teams
                          .filter((t) => t.id !== team.id)
                          .map((t) => (
                            <option key={t.id} value={t.id}>
                              {t.name}
                            </option>
                          ))}
                        <option value="unassigned">Takımsız</option>
                      </select>
                      <button
                        onClick={() => handleMove(team.id)}
                        disabled={selected.size === 0 || target === "" || isMoving === team.id}
                        style={{
                          padding:      "6px 12px",
                          borderRadius: 6,
                          border:       "none",
                          background:   selected.size === 0 || target === "" ? "#e2e8f0" : "#0f172a",
                          color:        "#ffffff",
                          fontSize:     12,
                          fontWeight:   500,
                          cursor:       selected.size === 0 || target === "" ? "not-allowed" : "pointer",
                        }}
                      >
                        {isMoving === team.id ? "Taşınıyor..." : "Taşı"}
                      </button>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
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