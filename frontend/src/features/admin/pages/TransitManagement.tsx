import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getAllTransitStops, getAllTransitRoutes } from "../../../shared/api/transitService";
import { getUserLookup } from "../../../shared/api/userLookupService";
import type { TransitStopResponseDto, TransitRouteResponseDto } from "../../map/transit/types";

interface UserLookupEntry {
  id:    number;
  email: string;
}

export function TransitManagement() {
  const [stops,      setStops]      = useState<TransitStopResponseDto[]>([]);
  const [routes,     setRoutes]     = useState<TransitRouteResponseDto[]>([]);
  const [users,      setUsers]      = useState<UserLookupEntry[]>([]);
  const [isLoading,  setIsLoading]  = useState(true);
  const [error,      setError]      = useState<string | null>(null);

  const { apiFetch } = useAuth();

  async function fetchAll() {
    setIsLoading(true);
    setError(null);
    try {
      const [stopRes, routeRes, userRes] = await Promise.all([
        getAllTransitStops(apiFetch),
        getAllTransitRoutes(apiFetch),
        getUserLookup(apiFetch),
      ]);

      if (!stopRes.ok || !routeRes.ok || !userRes.ok) {
        setError("Veriler yüklenemedi.");
        return;
      }

      setStops(await stopRes.json());
      setRoutes(await routeRes.json());
      setUsers(await userRes.json());
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchAll(); }, []);

  const emailById     = new Map(users.map((u) => [u.id, u.email]));
  const routeNameById = new Map(routes.map((r) => [r.id, r.name]));
  const stopCountByRouteId = stops.reduce<Record<number, number>>((acc, s) => {
    acc[s.transitRouteId] = (acc[s.transitRouteId] ?? 0) + 1;
    return acc;
  }, {});

  return (
    <div>
      <h1 style={{ fontSize: 22, fontWeight: 600, color: "#0f172a", marginBottom: 4 }}>
        Ulaşım Yönetimi
      </h1>
      <p style={{ fontSize: 14, color: "#64748b", marginBottom: 24 }}>
        Sistemdeki durakları ve güzergahları görüntüleyin.
      </p>

      {isLoading && <p style={{ color: "#94a3b8", fontSize: 13 }}>Yükleniyor...</p>}
      {error     && <p style={{ color: "#ef4444", fontSize: 13 }}>{error}</p>}

      {!isLoading && !error && (
        <>
          <h2 style={{ fontSize: 15, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
            Duraklar
          </h2>
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden", marginBottom: 32 }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>ID</th>
                  <th style={thStyle}>İsim</th>
                  <th style={thStyle}>Güzergah</th>
                  <th style={thStyle}>Ekleyen</th>
                  <th style={thStyle}>Oluşturma Tarihi</th>
                </tr>
              </thead>
              <tbody>
                {stops.length === 0 ? (
                  <tr>
                    <td colSpan={5} style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
                      Henüz durak eklenmemiş.
                    </td>
                  </tr>
                ) : (
                  stops.map((stop) => (
                    <tr key={stop.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                      <td style={tdStyle}>{stop.id}</td>
                      <td style={tdStyle}>{stop.name}</td>
                      <td style={tdStyle}>{routeNameById.get(stop.transitRouteId) ?? "—"}</td>
                      <td style={tdStyle}>{emailById.get(stop.userId) ?? "—"}</td>
                      <td style={tdStyle}>{new Date(stop.createdDate).toLocaleString("tr-TR")}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          <h2 style={{ fontSize: 15, fontWeight: 600, color: "#0f172a", marginBottom: 12 }}>
            Güzergahlar
          </h2>
          <div style={{ background: "#ffffff", borderRadius: 12, border: "1px solid #e2e8f0", overflow: "hidden" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ background: "#f8fafc" }}>
                  <th style={thStyle}>ID</th>
                  <th style={thStyle}>İsim</th>
                  <th style={thStyle}>Renk</th>
                  <th style={thStyle}>Durak Sayısı</th>
                  <th style={thStyle}>Ekleyen</th>
                  <th style={thStyle}>Oluşturma Tarihi</th>
                </tr>
              </thead>
              <tbody>
                {routes.length === 0 ? (
                  <tr>
                    <td colSpan={6} style={{ padding: 24, textAlign: "center", color: "#94a3b8", fontSize: 13 }}>
                      Henüz güzergah eklenmemiş.
                    </td>
                  </tr>
                ) : (
                  routes.map((route) => (
                    <tr key={route.id} style={{ borderTop: "1px solid #f1f5f9" }}>
                      <td style={tdStyle}>{route.id}</td>
                      <td style={tdStyle}>{route.name}</td>
                      <td style={tdStyle}>
                        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                          <div style={{ width: 12, height: 12, borderRadius: "50%", background: route.color }} />
                          <span style={{ fontFamily: "monospace", fontSize: 11 }}>{route.color}</span>
                        </div>
                      </td>
                      <td style={tdStyle}>{stopCountByRouteId[route.id] ?? 0}</td>
                      <td style={tdStyle}>{route.userId !== null ? (emailById.get(route.userId) ?? "—") : "—"}</td>
                      <td style={tdStyle}>{new Date(route.createdDate).toLocaleString("tr-TR")}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding:       "10px 16px",
  textAlign:     "left",
  fontSize:      11,
  fontWeight:    600,
  color:         "#64748b",
  letterSpacing: ".5px",
};

const tdStyle: React.CSSProperties = {
  padding: "10px 16px",
  color:   "#374151",
};