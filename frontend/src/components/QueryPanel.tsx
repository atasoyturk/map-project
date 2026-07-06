import { useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";

interface DrawingRow {
  id:          number;
  name:        string;
  color:       string;
  type:        string;
}

interface QueryPanelProps {
  onClose: () => void;
}

const TYPE_LABEL: Record<string, string> = {
  point:   "Nokta",
  line:    "Çizgi",
  polygon: "Poligon",
};

export function QueryPanel({ onClose }: QueryPanelProps) {
  const [rows,      setRows]      = useState<DrawingRow[]>([]);
  const [name,      setName]      = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate,   setEndDate]   = useState("");
  const [sortBy,    setSortBy]    = useState("creationDate");
  const [sortOrder, setSortOrder] = useState("asc");
  const [isLoading, setIsLoading] = useState(false);

  const { apiFetch } = useAuth();

  async function fetchData() {
    setIsLoading(true);
    try {
      const params = new URLSearchParams();
      if (name)      params.set("name",      name);
      if (startDate) params.set("startDate", startDate);
      if (endDate)   params.set("endDate",   endDate);
      params.set("sortBy",    sortBy);
      params.set("sortOrder", sortOrder);

      const res = await apiFetch(`/api/drawings/search?${params.toString()}`);
      if (!res.ok) return;

      const data = await res.json();

      const combined: DrawingRow[] = [
        ...data.points.map((p: any)   => ({ ...p, type: "point" })),
        ...data.lines.map((l: any)    => ({ ...l, type: "line" })),
        ...data.polygons.map((p: any) => ({ ...p, type: "polygon" })),
      ];

      setRows(combined);
    } catch {
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchData(); }, []);

  // Filtre/sort happened, refetch data
  useEffect(() => { fetchData(); }, [name, startDate, endDate, sortBy, sortOrder]);

  function toggleSort(col: string) {
    if (sortBy === col) setSortOrder((p) => p === "asc" ? "desc" : "asc");
    else { setSortBy(col); setSortOrder("asc"); }
  }

  function SortIcon({ col }: { col: string }) {
    if (sortBy !== col) return <span style={{ color: "#cbd5e1" }}>↕</span>;
    return <span style={{ color: "#6366f1" }}>{sortOrder === "asc" ? "↑" : "↓"}</span>;
  }

  return (
    <div style={{
      position: "fixed", bottom: 0, left: 0, right: 0,
      height: 320, background: "#ffffff", zIndex: 900,
      borderTop: "1px solid #e2e8f0",
      boxShadow: "0 -4px 24px rgba(0,0,0,0.08)",
      display: "flex", flexDirection: "column",
    }}>
      {/* Header + filters */}
      <div style={{
        display: "flex", alignItems: "center", gap: 12,
        padding: "10px 16px", borderBottom: "1px solid #f1f5f9",
        flexWrap: "wrap",
      }}>
        <span style={{ fontSize: 13, fontWeight: 600, color: "#0f172a", marginRight: 8 }}>
          Sorgulama Paneli
        </span>

        <input
          type="text"
          placeholder="İsim ara..."
          value={name}
          onChange={(e) => setName(e.target.value)}
          style={{
            padding: "5px 10px", borderRadius: 7, border: "1px solid #e2e8f0",
            fontSize: 12, color: "#0f172a", outline: "none", width: 160,
          }}
        />

        <input
          type="date"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
          style={{
            padding: "5px 10px", borderRadius: 7, border: "1px solid #e2e8f0",
            fontSize: 12, color: "#0f172a", outline: "none",
          }}
        />

        <input
          type="date"
          value={endDate}
          onChange={(e) => setEndDate(e.target.value)}
          style={{
            padding: "5px 10px", borderRadius: 7, border: "1px solid #e2e8f0",
            fontSize: 12, color: "#0f172a", outline: "none",
          }}
        />

        <button
          onClick={onClose}
          style={{
            marginLeft: "auto", padding: "5px 12px", borderRadius: 7,
            border: "1px solid #e2e8f0", background: "transparent",
            color: "#64748b", fontSize: 12, cursor: "pointer",
          }}
        >
          Kapat
        </button>
      </div>

      {/* TAble */}
      <div style={{ flex: 1, overflowY: "auto" }}>
        {isLoading ? (
          <div style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
            Yükleniyor...
          </div>
        ) : rows.length === 0 ? (
          <div style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
            Sonuç bulunamadı.
          </div>
        ) : (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
            <thead>
              <tr style={{ background: "#f8fafc", position: "sticky", top: 0 }}>
                <th style={thStyle} onClick={() => toggleSort("name")}>
                  İsim <SortIcon col="name" />
                </th>
                <th style={thStyle}>Tip</th>
                <th style={thStyle}>Renk</th>
                <th style={thStyle} onClick={() => toggleSort("creationDate")}>
                  Eklenme Tarihi <SortIcon col="creationDate" />
                </th>
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={`${row.type}-${row.id}`} style={{ borderBottom: "1px solid #f1f5f9" }}>
                  <td style={tdStyle}>{row.name || "—"}</td>
                  <td style={tdStyle}>{TYPE_LABEL[row.type]}</td>
                  <td style={tdStyle}>
                    <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                      <div style={{ width: 12, height: 12, borderRadius: "50%", background: row.color }} />
                      <span style={{ fontFamily: "monospace", fontSize: 11 }}>{row.color}</span>
                    </div>
                  </td>
                  <td style={tdStyle}>—</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: "8px 16px", textAlign: "left",
  fontSize: 11, fontWeight: 600, color: "#64748b",
  letterSpacing: ".5px", cursor: "pointer",
  borderBottom: "1px solid #e2e8f0",
};

const tdStyle: React.CSSProperties = {
  padding: "8px 16px", color: "#374151",
};