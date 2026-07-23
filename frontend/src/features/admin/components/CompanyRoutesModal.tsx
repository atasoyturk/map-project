import { useState, useEffect } from "react";
import { useAuth } from "../../auth/context/AuthContext";
import { getAllTransitRoutes } from "../../../shared/api/transitService";
import {
  getRoutesByCompany, assignRouteToCompany, removeRouteFromCompany,
} from "../../../shared/api/companyService";

interface RouteOption {
  id:   number;
  name: string;
}

interface CompanyRoutesModalProps {
  companyId:   number;
  companyName: string;
  onClose:     () => void;
}

export function CompanyRoutesModal({ companyId, companyName, onClose }: CompanyRoutesModalProps) {
  const { apiFetch } = useAuth();

  const [allRoutes,      setAllRoutes]      = useState<RouteOption[]>([]);
  const [assignedRouteIds, setAssignedRouteIds] = useState<Set<number>>(new Set());
  const [isLoading,      setIsLoading]      = useState(true);
  const [busyRouteId,    setBusyRouteId]    = useState<number | null>(null);
  const [error,          setError]          = useState<string | null>(null);

  async function fetchData() {
    setIsLoading(true);
    try {
      const [allRes, assignedRes] = await Promise.all([
        getAllTransitRoutes(apiFetch),
        getRoutesByCompany(apiFetch, companyId),
      ]);

      if (allRes.ok) setAllRoutes(await allRes.json());
      if (assignedRes.ok) {
        const assigned: RouteOption[] = await assignedRes.json();
        setAssignedRouteIds(new Set(assigned.map((r) => r.id)));
      }
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => { fetchData(); }, [companyId]);

  async function toggleRoute(routeId: number, isAssigned: boolean) {
    setBusyRouteId(routeId);
    setError(null);
    try {
      const res = isAssigned
        ? await removeRouteFromCompany(apiFetch, companyId, routeId)
        : await assignRouteToCompany(apiFetch, companyId, routeId);

      if (!res.ok) { setError("İşlem başarısız oldu."); return; }

      setAssignedRouteIds((prev) => {
        const next = new Set(prev);
        isAssigned ? next.delete(routeId) : next.add(routeId);
        return next;
      });
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setBusyRouteId(null);
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
          padding: "28px 32px", width: 420, maxHeight: "70vh",
          display: "flex", flexDirection: "column",
          boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
        }}
      >
        <h2 style={{ fontSize: 18, fontWeight: 600, color: "#0f172a", margin: "0 0 4px" }}>
          Güzergahlar
        </h2>
        <p style={{ fontSize: 13, color: "#64748b", margin: "0 0 16px" }}>
          {companyName} şirketine atanmış güzergahları yönetin.
        </p>

        {error && <p style={{ fontSize: 12, color: "#dc2626", margin: "0 0 12px" }}>{error}</p>}

        {isLoading ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>Yükleniyor...</p>
        ) : allRoutes.length === 0 ? (
          <p style={{ fontSize: 13, color: "#94a3b8" }}>Henüz güzergah yok.</p>
        ) : (
          <div style={{ overflowY: "auto", display: "flex", flexDirection: "column", gap: 6, marginBottom: 20 }}>
            {allRoutes.map((route) => {
              const isAssigned = assignedRouteIds.has(route.id);
              return (
                <label
                  key={route.id}
                  style={{
                    display: "flex", alignItems: "center", gap: 8,
                    padding: "8px 10px", borderRadius: 8,
                    border: "1px solid #e2e8f0", fontSize: 13, color: "#374151",
                    cursor: busyRouteId === route.id ? "not-allowed" : "pointer",
                    opacity: busyRouteId === route.id ? 0.6 : 1,
                  }}
                >
                  <input
                    type="checkbox"
                    checked={isAssigned}
                    disabled={busyRouteId === route.id}
                    onChange={() => toggleRoute(route.id, isAssigned)}
                  />
                  <span style={{ flex: 1 }}>{route.name}</span>
                </label>
              );
            })}
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            padding: "9px 0", borderRadius: 8,
            border: "1px solid #e2e8f0", background: "#f8fafc",
            color: "#64748b", fontSize: 13, fontWeight: 500, cursor: "pointer",
          }}
        >
          Kapat
        </button>
      </div>
    </div>
  );
}