import { useEffect, useRef, useState } from "react";
import type VectorLayer  from "ol/layer/Vector";
import type VectorSource from "ol/source/Vector";
import { Style } from "ol/style";
import { useAuth } from "../../../auth/context/AuthContext";
import {
  createTransitRoute, deleteTransitRoute,
  getTransitRouteDetail, reorderTransitStops, generateTransitRoute, clearTransitRoute,
} from "../../../../shared/api/transitService";
import {
  getAllCompanyCategories, getAllCompanies,
} from "../../../../shared/api/companyService";
import { buildRouteStyle } from "../../../../utils/mapStyle";
import type { TransitRouteResponseDto, TransitRouteDetailDto, TransitStopResponseDto } from "../types";
import type { CompanyCategoryResponseDto, CompanyResponseDto } from "../../../company/types";

type FilterMode = "" | "unassigned";

 interface RouteManagementPanelProps {
  routes:            TransitRouteResponseDto[];
  reloadRoutes:      () => void;
  onClose:           () => void;
  onAddStopToRoute:  (routeId: number) => void;
  routeLineLayer:    VectorLayer<VectorSource> | null;
}

export function RouteManagementPanel({
  routes, reloadRoutes, onClose, onAddStopToRoute, routeLineLayer,
}: RouteManagementPanelProps) {
  const { apiFetch } = useAuth();

  const [generatingRouteId, setGeneratingRouteId] = useState<number | null>(null);
  const [routeGenError,     setRouteGenError]     = useState<string | null>(null);
  const [hiddenRouteIds,    setHiddenRouteIds]     = useState<Set<number>>(new Set());

  const [categories, setCategories] = useState<CompanyCategoryResponseDto[]>([]);
  const [companies,  setCompanies]  = useState<CompanyResponseDto[]>([]);

  const [selectedCategoryId, setSelectedCategoryId] = useState<number | FilterMode>("");
  const [selectedCompanyId,  setSelectedCompanyId]  = useState<number | FilterMode>("");
  const [filteredRouteIds,   setFilteredRouteIds]   = useState<Set<number> | null>(null);
  const [isFiltering,        setIsFiltering]        = useState(false);

  const [newName,  setNewName]  = useState("");
  const [newColor, setNewColor] = useState("#3b82f6");
  const [isSaving, setIsSaving] = useState(false);
  const [error,    setError]    = useState<string | null>(null);

  const [expandedId,   setExpandedId]   = useState<number | null>(null);
  const [detailById,   setDetailById]   = useState<Record<number, TransitRouteDetailDto>>({});
  const [isReordering, setIsReordering] = useState<number | null>(null);

  const dragItemId     = useRef<number | null>(null);
  const dragOverItemId = useRef<number | null>(null);

  async function handleCreateRoute() {
    if (!newName.trim() || isSaving) return;
    setIsSaving(true);
    setError(null);
    try {
      const res = await createTransitRoute(apiFetch, { name: newName.trim(), color: newColor });
      if (!res.ok) { setError("Güzergah oluşturulamadı."); return; }
      setNewName("");
      reloadRoutes();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDeleteRoute(id: number) {
    setError(null);
    try {
      const res = await deleteTransitRoute(apiFetch, id);
      if (!res.ok) {
        setError(res.status === 400 ? "Bu güzergaha bağlı duraklar var, önce onları taşıyın/silin." : "Silme başarısız.");
        return;
      }
      if (expandedId === id) setExpandedId(null);
      reloadRoutes();
    } catch {
      setError("Sunucuya bağlanılamadı.");
    }
  }

  async function handleGenerateRoute(routeId: number) {
    setRouteGenError(null);
    setGeneratingRouteId(routeId);
    try {
      const res = await generateTransitRoute(apiFetch, routeId);
      if (!res.ok) {
        const message = res.status === 400 ? await res.text() : "Rota oluşturulamadı.";
        setRouteGenError(message);
        return;
      }
      reloadRoutes();
    } catch {
      setRouteGenError("Sunucuya bağlanılamadı.");
    } finally {
      setGeneratingRouteId(null);
    }
  }

  async function handleClearRoute(routeId: number) {
    setRouteGenError(null);
    setGeneratingRouteId(routeId);
    try {
      const res = await clearTransitRoute(apiFetch, routeId);
      if (!res.ok) {
        setRouteGenError("Rota temizlenemedi.");
        return;
      }
      reloadRoutes();
    } catch {
      setRouteGenError("Sunucuya bağlanılamadı.");
    } finally {
      setGeneratingRouteId(null);
    }
  }

  function toggleRouteVisibility(routeId: number) {
    setHiddenRouteIds((prev) => {
      const next = new Set(prev);
      if (next.has(routeId)) next.delete(routeId);
      else next.add(routeId);
      return next;
    });
  }

  useEffect(() => {
    if (!routeLineLayer) return;
    const source = routeLineLayer.getSource();
    if (!source) return;

    for (const route of routes) {
      const feature = source.getFeatureById(route.id);
      if (!feature) continue;

      feature.setStyle(
        hiddenRouteIds.has(route.id) ? new Style() : buildRouteStyle(route.color)
      );
    }
  }, [routeLineLayer, routes, hiddenRouteIds]);

  async function toggleExpand(routeId: number) {
    if (expandedId === routeId) { setExpandedId(null); return; }
    setExpandedId(routeId);
    await loadDetail(routeId);
  }

  async function loadDetail(routeId: number) {
    try {
      const res = await getTransitRouteDetail(apiFetch, routeId);
      if (!res.ok) return;
      const data: TransitRouteDetailDto = await res.json();
      setDetailById((prev) => ({ ...prev, [routeId]: data }));
    } catch { }
  }

  function handleDragStart(stopId: number) {
    dragItemId.current = stopId;
  }

  function handleDragOver(e: React.DragEvent, stopId: number) {
    e.preventDefault();
    dragOverItemId.current = stopId;
  }

  useEffect(() => {
    (async () => {
      try {
        const [catRes, compRes] = await Promise.all([
          getAllCompanyCategories(apiFetch),
          getAllCompanies(apiFetch),
        ]);
        if (catRes.ok) setCategories(await catRes.json());
        if (compRes.ok) setCompanies(await compRes.json());
      } catch { }
    })();
  }, []);

  useEffect(() => {
    if (selectedCategoryId === "" && selectedCompanyId === "") {
      setFilteredRouteIds(new Set());
      return;
    }

    let cancelled = false;
    setIsFiltering(true);

    (async () => {
      try {
        const idSets: Set<number>[] = [];

        if (selectedCategoryId === "unassigned" || selectedCompanyId === "unassigned") {
          const res = await apiFetch("/api/company/routes/unassigned");
          if (res.ok) {
            const data: TransitRouteResponseDto[] = await res.json();
            idSets.push(new Set(data.map((r) => r.id)));
          }
        } else {
          if (selectedCategoryId !== "") {
            const res = await apiFetch(`/api/company-category/${selectedCategoryId}/routes`);
            if (res.ok) {
              const data: TransitRouteResponseDto[] = await res.json();
              idSets.push(new Set(data.map((r) => r.id)));
            }
          }
          if (selectedCompanyId !== "") {
            const res = await apiFetch(`/api/company/${selectedCompanyId}/routes`);
            if (res.ok) {
              const data: TransitRouteResponseDto[] = await res.json();
              idSets.push(new Set(data.map((r) => r.id)));
            }
          }
        }

        if (cancelled) return;

        const intersection = idSets.reduce((acc, set) =>
          acc === null ? set : new Set([...acc].filter((id) => set.has(id))), null as Set<number> | null);

        setFilteredRouteIds(intersection ?? new Set());
      } catch {
        if (!cancelled) setFilteredRouteIds(new Set());
      } finally {
        if (!cancelled) setIsFiltering(false);
      }
    })();

    return () => { cancelled = true; };
  }, [selectedCategoryId, selectedCompanyId]);

  function handleCategoryChange(value: string) {
    if (value === "unassigned") {
      setSelectedCategoryId("unassigned");
      setSelectedCompanyId("");
    } else {
      setSelectedCategoryId(value === "" ? "" : Number(value));
    }
  }

  function handleCompanyChange(value: string) {
    if (value === "unassigned") {
      setSelectedCompanyId("unassigned");
      setSelectedCategoryId("");
    } else {
      setSelectedCompanyId(value === "" ? "" : Number(value));
    }
  }

  const visibleRoutes = filteredRouteIds
    ? routes.filter((r) => filteredRouteIds.has(r.id))
    : [];


  async function handleDrop(routeId: number) {
    const fromId = dragItemId.current;
    const toId   = dragOverItemId.current;
    dragItemId.current     = null;
    dragOverItemId.current = null;
    if (fromId == null || toId == null || fromId === toId) return;

    const detail = detailById[routeId];
    if (!detail) return;

    const stops     = [...detail.stops];
    const fromIndex = stops.findIndex((s) => s.id === fromId);
    const toIndex   = stops.findIndex((s) => s.id === toId);
    if (fromIndex === -1 || toIndex === -1) return;

    const [moved] = stops.splice(fromIndex, 1);
    stops.splice(toIndex, 0, moved);

    // İyimser güncelleme — sürükleme sonrası liste anında yeni sırayla görünsün
    setDetailById((prev) => ({ ...prev, [routeId]: { ...detail, stops } }));
    setIsReordering(routeId);

    try {
      const res = await reorderTransitStops(apiFetch, routeId, stops.map((s) => s.id));
      if (!res.ok) { await loadDetail(routeId); setError("Sıralama kaydedilemedi."); return; }
    } catch {
      await loadDetail(routeId);
      setError("Sunucuya bağlanılamadı.");
    } finally {
      setIsReordering(null);
    }
  }

  return (
    <div style={{
      position:     "fixed",
      top:          70, bottom: 100, right: 16,
      width:        340,
      background:   "#ffffff",
      borderRadius: 12,
      border:       "1px solid #e2e8f0",
      boxShadow:    "0 8px 32px rgba(0,0,0,0.15)",
      zIndex:       950,
      display:      "flex",
      flexDirection:"column",
      overflow:     "hidden",
    }}>
      <div style={{
        display: "flex", alignItems: "center", justifyContent: "space-between",
        padding: "12px 16px", borderBottom: "1px solid #f1f5f9",
      }}>
        <span style={{ fontSize: 14, fontWeight: 600, color: "#0f172a" }}>Güzergah Yönetimi</span>
        <button
          onClick={onClose}
          style={{
            border: "none", background: "transparent",
            color: "#94a3b8", fontSize: 16, cursor: "pointer", lineHeight: 1,
          }}
        >
          ✕
        </button>
      </div>

      <div style={{ padding: "12px 16px", borderBottom: "1px solid #f1f5f9", display: "flex", gap: 8 }}>
        <select
          value={selectedCategoryId}
          onChange={(e) => handleCategoryChange(e.target.value)}
          style={{
            flex: 1, padding: "7px 8px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 12, color: "#0f172a",
          }}
        >
          <option value="">Kategori seçin...</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
          <option value="unassigned">— Atanmamış —</option>
        </select>
        <select
          value={selectedCompanyId}
          onChange={(e) => handleCompanyChange(e.target.value)}
          style={{
            flex: 1, padding: "7px 8px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 12, color: "#0f172a",
          }}
        >
          <option value="">Şirket seçin...</option>
          {companies.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
          <option value="unassigned">— Atanmamış —</option>
        </select>
      </div>

      <div style={{ padding: "12px 16px", borderBottom: "1px solid #f1f5f9", display: "flex", gap: 8 }}>
        <input
          type="text"
          placeholder="Yeni güzergah adı..."
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          style={{
            flex: 1, padding: "7px 10px", borderRadius: 8,
            border: "1px solid #e2e8f0", fontSize: 13, color: "#0f172a", outline: "none",
          }}
        />
        <input
          type="color"
          value={newColor}
          onChange={(e) => setNewColor(e.target.value)}
          style={{ width: 36, height: 32, padding: 0, border: "1px solid #e2e8f0", borderRadius: 8, cursor: "pointer" }}
        />
        <button
          onClick={handleCreateRoute}
          disabled={isSaving || !newName.trim()}
          style={{
            padding: "7px 12px", borderRadius: 8, border: "none",
            background: "#0f172a", color: "#ffffff", fontSize: 13, fontWeight: 500,
            cursor: isSaving ? "not-allowed" : "pointer", opacity: isSaving ? 0.6 : 1,
          }}
        >
          Ekle
        </button>
      </div>

      {error && (
        <p style={{ fontSize: 12, color: "#dc2626", padding: "8px 16px 0", margin: 0 }}>{error}</p>
      )}

      <div style={{ flex: 1, overflowY: "auto", padding: 12 }}>
        {isFiltering ? (
          <p style={{ fontSize: 13, color: "#94a3b8", textAlign: "center", padding: "20px 0" }}>
            Yükleniyor...
          </p>
        ) : selectedCategoryId === "" && selectedCompanyId === "" ? (
          <p style={{ fontSize: 13, color: "#94a3b8", textAlign: "center", padding: "20px 0" }}>
            Güzergahları görüntülemek için bir kategori veya şirket seçin.
          </p>
        ) : visibleRoutes.length === 0 ? (
          <p style={{ fontSize: 13, color: "#94a3b8", textAlign: "center", padding: "20px 0" }}>
            Bu filtreye uyan güzergah yok.
          </p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {visibleRoutes.map((route) => {
              const isExpanded = expandedId === route.id;
              const detail     = detailById[route.id];
              const stopCount  = detail?.stops.length;

              return (
                <div
                  key={route.id}
                  style={{ border: "1px solid #e2e8f0", borderRadius: 10, overflow: "hidden" }}
                >
                  <div style={{
                    display: "flex", alignItems: "center", gap: 8,
                    padding: "10px 12px", cursor: "pointer",
                  }}
                    onClick={() => toggleExpand(route.id)}
                  >
                    <div style={{ width: 12, height: 12, borderRadius: "50%", background: route.color, flexShrink: 0 }} />
                    <span style={{ flex: 1, fontSize: 13, fontWeight: 500, color: "#0f172a" }}>
                      {route.name}
                    </span>
                    {route.routeWktGeometry && (
                      <input
                        type="checkbox"
                        defaultChecked
                        onClick={(e) => e.stopPropagation()}
                        onChange={() => toggleRouteVisibility(route.id)}
                        title="Haritada göster/gizle"
                        style={{ cursor: "pointer" }}
                      />
                    )}
                    {stopCount !== undefined && (
                      <span style={{ fontSize: 11, color: "#94a3b8" }}>{stopCount} durak</span>
                    )}
                    <span style={{ fontSize: 11, color: "#94a3b8" }}>{isExpanded ? "▲" : "▼"}</span>
                  </div>

                  {isExpanded && (
                    <div style={{ borderTop: "1px solid #f1f5f9", padding: "10px 12px", background: "#f8fafc" }}>
                      {!detail ? (
                        <p style={{ fontSize: 12, color: "#94a3b8", margin: 0 }}>Yükleniyor...</p>
                      ) : detail.stops.length === 0 ? (
                        <p style={{ fontSize: 12, color: "#94a3b8", margin: 0 }}>Bu güzergahta durak yok.</p>
                      ) : (
                        <div style={{ display: "flex", flexDirection: "column", gap: 4, marginBottom: 10 }}>
                          {detail.stops.map((stop: TransitStopResponseDto, i: number) => (
                            <div
                              key={stop.id}
                              draggable
                              onDragStart={() => handleDragStart(stop.id)}
                              onDragOver={(e) => handleDragOver(e, stop.id)}
                              onDrop={() => handleDrop(route.id)}
                              style={{
                                display: "flex", alignItems: "center", gap: 8,
                                padding: "6px 8px", borderRadius: 6,
                                background: "#ffffff", border: "1px solid #e2e8f0",
                                fontSize: 12, color: "#374151",
                                cursor: "grab",
                                opacity: isReordering === route.id ? 0.6 : 1,
                              }}
                            >
                              <span style={{ color: "#cbd5e1", fontSize: 14 }}>⋮⋮</span>
                              <span style={{ width: 18, textAlign: "center", color: "#94a3b8", fontSize: 11 }}>{i + 1}</span>
                              <span style={{ flex: 1 }}>{stop.name}</span>
                            </div>
                          ))}
                        </div>
                      )}

                      {routeGenError && (
                        <p style={{ fontSize: 11, color: "#dc2626", margin: "0 0 8px" }}>{routeGenError}</p>
                      )}
                      <div style={{ display: "flex", gap: 6 }}>
                        <button
                          onClick={() => onAddStopToRoute(route.id)}
                          style={{
                            flex: 1, padding: "6px 0", borderRadius: 6,
                            border: "1px solid rgba(59,130,246,.3)", background: "rgba(59,130,246,.05)",
                            color: "#3b82f6", fontSize: 12, fontWeight: 500, cursor: "pointer",
                          }}
                        >
                          Durak Ekle
                        </button>
                        <button
                          onClick={() => handleGenerateRoute(route.id)}
                          disabled={(stopCount ?? 0) < 2 || generatingRouteId === route.id}
                          style={{
                            flex: 1, padding: "6px 0", borderRadius: 6,
                            border: "1px solid rgba(16,185,129,.3)", background: "rgba(16,185,129,.05)",
                            color: "#10b981", fontSize: 12, fontWeight: 500,
                            cursor: (stopCount ?? 0) < 2 ? "not-allowed" : "pointer",
                            opacity: (stopCount ?? 0) < 2 ? 0.5 : 1,
                          }}
                        >
                          {generatingRouteId === route.id ? "Oluşturuluyor..." : "Rota Oluştur"}
                        </button>
                        {route.routeWktGeometry && (
                          <button
                            onClick={() => handleClearRoute(route.id)}
                            disabled={generatingRouteId === route.id}
                            style={{
                              padding: "6px 10px", borderRadius: 6,
                              border: "1px solid #e2e8f0", background: "#f8fafc",
                              color: "#64748b", fontSize: 12, fontWeight: 500, cursor: "pointer",
                            }}
                          >
                            Rotayı Temizle
                          </button>
                        )}
                        <button
                          onClick={() => handleDeleteRoute(route.id)}
                          style={{
                            padding: "6px 12px", borderRadius: 6,
                            border: "1px solid rgba(239,68,68,.3)", background: "rgba(239,68,68,.05)",
                            color: "#ef4444", fontSize: 12, fontWeight: 500, cursor: "pointer",
                          }}
                        >
                          Sil
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}