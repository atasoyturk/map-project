import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth }    from "../../../auth/context/AuthContext";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { buildStyle } from "../../../../shared/utils/mapStyle";
import { useDrawing, type DrawingLayers } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { useAnalysis } from "../hooks/useAnalysis";
import { AttributeModal } from "./AttributeModal";
import { Toast }      from "../../../../shared/components/Toast";
import type { DrawType, PendingGeometry } from "../types";
import { createGeoFeature, validateGeometry } from "../api/geoService";
import { BottomToolbar } from "./BottomToolbar";
import { NavDropdown } from "./NavDropdown";
import { Z } from "../../../../shared/components/ui/zIndex";

interface NavbarProps {
  map:               Map | null;
  analysisActive:    boolean;
  onAnalysisChange:  (v: boolean) => void;
  activeType:        DrawType | null;
  onActiveTypeChange:(v: DrawType | null) => void;
  onLayersReady?:    (layers: DrawingLayers) => void;
  queryPanelOpen:    boolean;
  onQueryPanelToggle:() => void;
  layerControlOpen:  boolean;
  onLayerControlToggle: () => void;
  heatmapActive:    boolean;
  onHeatmapToggle:  () => void;
  poiDrawActive:    boolean;
  onPoiDrawChange:  (active: boolean) => void;
  poiFormOpen:      boolean;
  locAnalysisActive: boolean;
  onLocAnalysisToggle: () => void;
  onLocAnalysisPolygonReady?: (wkt: string, feature: any) => void;
  stopDrawActive:        boolean;
  onStopDrawChange:      (active: boolean) => void;
  stopFormOpen:          boolean;
  routeManagementOpen:   boolean;
  onRouteManagementToggle: () => void;
  tripPlanningOpen:        boolean;
  onTripPlanningToggle:    () => void;
  thematicMapOpen:         boolean;
  onThematicMapToggle:     () => void;
}


interface ToastState {
  message: string;
  type:    "success" | "error";
}

export function Navbar({
  map,
  analysisActive,
  onAnalysisChange,
  activeType,
  onActiveTypeChange,
  onLayersReady,
  queryPanelOpen,
  onQueryPanelToggle,
  layerControlOpen,
  onLayerControlToggle,
  heatmapActive,
  onHeatmapToggle,
  poiDrawActive,
  onPoiDrawChange,
  poiFormOpen,
  locAnalysisActive,
  onLocAnalysisToggle,
  onLocAnalysisPolygonReady,
  stopDrawActive,
  onStopDrawChange,
  stopFormOpen,
  routeManagementOpen,
  onRouteManagementToggle,
  tripPlanningOpen,
  onTripPlanningToggle,
  thematicMapOpen,
  onThematicMapToggle,
}: NavbarProps) {

  const [pendingGeometry, setPendingGeometry] = useState<PendingGeometry | null>(null);
  const [isSaving,        setIsSaving]        = useState(false);
  const [toast,           setToast]           = useState<ToastState | null>(null);
  const [analysisResult,  setAnalysisResult]  = useState<number | null>(null);
  const [teamModeActive,  setTeamModeActive]  = useState(false);
  const [openDropdowns,   setOpenDropdowns]   = useState<Set<string>>(new Set());
  const [menuOpen,        setMenuOpen]        = useState(false);
  const navRef = useRef<HTMLElement | null>(null);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (navRef.current && !navRef.current.contains(e.target as Node)) {
        setMenuOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function handleDropdownOpenChange(label: string, open: boolean) {
    setOpenDropdowns((prev) => {
      const next = new Set(prev);
      open ? next.add(label) : next.delete(label);
      return next;
    });
  }

  const pointSourceRef   = useRef(new VectorSource());
  const lineSourceRef    = useRef(new VectorSource());
  const polygonSourceRef = useRef(new VectorSource());

  const { logout, apiFetch, hasPermission, teamId } = useAuth();
  const navigate = useNavigate();

  const canDraw =
    hasPermission("point_create") || hasPermission("line_create") || hasPermission("polygon_create");

  const isPlainUser = !canDraw;

  const canManagePoi           = hasPermission("poi_create");
  const canManageTransitStops  = hasPermission("transit_stop_create");
  const canManageTransitRoutes = hasPermission("transit_route_manage");
  const canAreaScan            = hasPermission("area_scan");
  const canLocationAnalysis    = hasPermission("location_analysis");
  const canViewHeatmap         = hasPermission("heatmap_view");
  
  const otherToolActive = !!activeType || analysisActive;
  const toolsLocked = !!pendingGeometry || heatmapActive || poiDrawActive || poiFormOpen || stopDrawActive || stopFormOpen;

  useFeatureLoader({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    apiFetch,
    buildStyle,
    viewMode: teamModeActive ? "team" : "own",
  });

  const { layers } = useDrawing({
    map,
    pointSource:   pointSourceRef.current,
    lineSource:    lineSourceRef.current,
    polygonSource: polygonSourceRef.current,
    activeType,
    onDrawEnd: async (pending) => {
      
      if (locAnalysisActive && pending.type === "Polygon") {
        // DashboardPage'e poligonu bildir (bu prop'u birazdan ekleyeceğiz)
        onLocAnalysisPolygonReady?.(pending.wkt, pending.feature);
        onActiveTypeChange(null);
        return;
      }
      
      pending.feature.setStyle(buildStyle("#3b82f6", ""));

      try {
        const res = await validateGeometry(apiFetch, pending.wkt);

        if (!res.ok) {
          const src =
            pending.type === "Point"      ? pointSourceRef.current   :
            pending.type === "LineString"  ? lineSourceRef.current    :
                                             polygonSourceRef.current;
          src.removeFeature(pending.feature);
          const message = await res.text();
          setToast({ message: `Hata: ${message}`, type: "error" });
          onActiveTypeChange(null);
          return;
        }

        setPendingGeometry(pending);
        onActiveTypeChange(null);
      } catch {
        setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
        onActiveTypeChange(null);
      }
    },
  });

  useEffect(() => {
    if (layers) onLayersReady?.(layers);
  }, [layers]);

  const { clear: clearAnalysis } = useAnalysis({
    map,
    active:   analysisActive,
    apiFetch,
    onResult: (count) => {
      setAnalysisResult(count);
      onAnalysisChange(false);
      setToast({
        message: `Analiz tamamlandı! Seçilen alan içinde ${count} adet öğe bulundu.`,
        type:    "success",
      });
    },
    onError: (msg) => setToast({ message: msg, type: "error" }),
  });

  async function handleModalSave(name: string, color: string) {
    if (!pendingGeometry) return;
    setIsSaving(true);
    try {
      const response = await createGeoFeature(apiFetch, pendingGeometry.type, {
        wktGeometry: pendingGeometry.wkt,
        name,
        color,
      });

      if (!response.ok) {
        if (response.status === 403) {
          const message = await response.text();
          setToast({ message: `Hata: ${message}`, type: "error" });
        } else {
          setToast({ message: "Kaydetme başarısız.", type: "error" });
        }
        const src =
          pendingGeometry.type === "Point"      ? pointSourceRef.current   :
          pendingGeometry.type === "LineString"  ? lineSourceRef.current    :
                                                   polygonSourceRef.current;
        src.removeFeature(pendingGeometry.feature);
        setPendingGeometry(null);
        return;
      }

      pendingGeometry.feature.setStyle(buildStyle(color, name));
      setToast({ message: "Başarıyla kaydedildi.", type: "success" });
      setPendingGeometry(null);
    } catch {
      setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setIsSaving(false);
    }
  }

  function handleSelect(type: DrawType) {
    setToast(null);
    onAnalysisChange(false);
    setAnalysisResult(null);
    clearAnalysis();
    onPoiDrawChange(false);
    onActiveTypeChange(activeType === type ? null : type);
  }

  function handleAnalysisToggle() {
    const next = !analysisActive;
    onAnalysisChange(next);
    onActiveTypeChange(null);
    onPoiDrawChange(false);
    if (!next) { clearAnalysis(); setAnalysisResult(null); }
    setToast(null);
  }

  function handlePoiButtonClick() {
    setToast(null);
    onAnalysisChange(false);
    setAnalysisResult(null);
    clearAnalysis();
    onActiveTypeChange(null);
    onPoiDrawChange(!poiDrawActive);
  }

  function handleModalCancel() {
    if (pendingGeometry) {
      const src =
        pendingGeometry.type === "Point"      ? pointSourceRef.current   :
        pendingGeometry.type === "LineString"  ? lineSourceRef.current    :
                                                 polygonSourceRef.current;
      src.removeFeature(pendingGeometry.feature);
    }
    setPendingGeometry(null);
    onActiveTypeChange(null);
  }

  function handleLogout() {
    pointSourceRef.current.clear();
    lineSourceRef.current.clear();
    polygonSourceRef.current.clear();
    logout();
    navigate("/login");
  }
  const isAdmin = hasPermission("admin_access");

  const isExpanded = menuOpen || !isPlainUser && (
    analysisActive || heatmapActive || poiDrawActive || poiFormOpen ||
    locAnalysisActive || queryPanelOpen || layerControlOpen || openDropdowns.size > 0
  );

  if (isPlainUser) {
    return (
      <nav
        ref={navRef as React.RefObject<HTMLElement>}
        style={{
          position: "fixed", top: 12, left: "50%", transform: "translateX(-50%)",
          zIndex: Z.toolbar,
          display:      "flex",
          alignItems:   "center",
          gap:          8,
          background:   "#030c21",
          border:       "1px solid rgba(255,255,255,.08)",
          borderRadius: 12,
          padding:      "8px 16px",
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            AtaGIS
          </span>
        </div>

        <button
          onClick={() => setMenuOpen((p) => !p)}
          style={{
            border: "none", background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500, cursor: "pointer", padding: 0,
          }}
        >
          Menü
        </button>

        <div style={{
          display: "flex", alignItems: "center", gap: 8,
          maxWidth:  menuOpen ? 500 : 0,
          opacity:   menuOpen ? 1 : 0,
          overflow: menuOpen ? "visible" : "hidden",
          transition: "max-width .6s ease, opacity .4s ease",
        }}>
        <button
          onClick={onTripPlanningToggle}
          style={{
            padding: "6px 14px", borderRadius: 8, border: "1px solid",
            borderColor: tripPlanningOpen ? "#22c55e" : "rgba(255,255,255,.15)",
            background:  tripPlanningOpen ? "rgba(34,197,94,.2)" : "transparent",
            color:       tripPlanningOpen ? "#86efac" : "#94a3b8",
            fontSize: 13, fontWeight: 500, cursor: "pointer", transition: "all .15s",
          }}
        >
          Yol Tarifi
        </button>

        <button
          onClick={onThematicMapToggle}
          style={{
            padding: "6px 14px", borderRadius: 8, border: "1px solid",
            borderColor: thematicMapOpen ? "#f59e0b" : "rgba(255,255,255,.15)",
            background:  thematicMapOpen ? "rgba(245,158,11,.2)" : "transparent",
            color:       thematicMapOpen ? "#fcd34d" : "#94a3b8",
            fontSize: 13, fontWeight: 500, cursor: "pointer", transition: "all .15s",
          }}
        >
          Hizmet Alanı
        </button>

        <button
          onClick={handleLogout}
          style={{
            padding: "6px 14px", borderRadius: 8,
            border: "1px solid rgba(255,255,255,.15)",
            background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500,
            cursor: "pointer", minWidth: 80,
          }}
        >
          Çıkış Yap
        </button>
        </div>
      </nav>
    );
  }

  return (
    <>
      <nav
        ref={navRef as React.RefObject<HTMLElement>}
        style={{
          position: "fixed", top: 12, left: "50%", transform: "translateX(-50%)",
          zIndex: Z.toolbar,
          display:      "flex",
          alignItems:   "center",
          gap:          8,
          background:   "#030c21",
          border:       "1px solid rgba(255,255,255,.08)",
          borderRadius: 12,
          padding:      "8px 16px",
        }}
      >

        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            AtaGIS
          </span>
        </div>

        <button
          onClick={() => setMenuOpen((p) => !p)}
          style={{
            border: "none", background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500, cursor: "pointer", padding: 0,
          }}
        >
          Menü
        </button>

        <div style={{
          display: "flex", alignItems: "center", gap: 10,
          maxWidth:  isExpanded ? 900 : 0,
          opacity:   isExpanded ? 1 : 0,
          overflow: isExpanded ? "visible" : "hidden",
          transition: "max-width .6s ease, opacity .4s ease",
        }}>
        {isAdmin && (
          <button
            onClick={() => navigate("/admin")}
            style={{
              padding: "6px 14px", borderRadius: 8,
              border: "1px solid rgba(255,255,255,.15)",
              background: "transparent", color: "#94a3b8",
              fontSize: 13, fontWeight: 500, cursor: "pointer", transition: "all .15s",
            }}
          >
            Panel
          </button>
        )}

        <div style={{ width: 1, height: 24, background: "rgba(255,255,255,.08)" }} />

        <NavDropdown
          label="Analiz"
          disabled={toolsLocked}
          hasActiveItem={analysisActive || locAnalysisActive || heatmapActive || thematicMapOpen}
          onOpenChange={(open) => handleDropdownOpenChange("Analiz", open)}
        >
          {canAreaScan && (
            <button onClick={handleAnalysisToggle} disabled={toolsLocked} style={navItemStyle(analysisActive, "#eab308", "#fde047", toolsLocked)}>
              Alan Tara
            </button>
          )}
          {canLocationAnalysis && (
            <button onClick={onLocAnalysisToggle} disabled={toolsLocked} style={navItemStyle(locAnalysisActive, "#ec4899", "#fbcfe8", toolsLocked)}>
              Konum Analizi
            </button>
          )}
          {canViewHeatmap && (
            <button
              onClick={onHeatmapToggle}
              disabled={!!pendingGeometry || poiDrawActive || poiFormOpen}
              style={navItemStyle(heatmapActive, "#ef4444", "#fca5a5", !!pendingGeometry || poiDrawActive || poiFormOpen)}
            >
              Isı Haritası
            </button>
          )}
          <button onClick={onThematicMapToggle} disabled={toolsLocked} style={navItemStyle(thematicMapOpen, "#f59e0b", "#fcd34d", toolsLocked)}>
            Hizmet Alanı
          </button>
        </NavDropdown>

        {analysisResult !== null && (
          <button
            onClick={() => { clearAnalysis(); setAnalysisResult(null); setToast(null); }}
            style={{
              padding: "6px 12px", borderRadius: 8,
              border: "1px solid rgba(234,179,8,.4)",
              background: "rgba(234,179,8,.1)",
              color: "#fde047", fontSize: 12, cursor: "pointer",
            }}
          >
            Temizle
          </button>
        )}

        {(canManageTransitStops || canManageTransitRoutes) && (
          <NavDropdown
            label="Güzergah"
            disabled={toolsLocked}
            hasActiveItem={stopDrawActive || routeManagementOpen || tripPlanningOpen}
            onOpenChange={(open) => handleDropdownOpenChange("Güzergah", open)}
          >
            {canManageTransitStops && (
              <button
                onClick={() => onStopDrawChange(!stopDrawActive)}
                disabled={!!pendingGeometry || heatmapActive || otherToolActive || poiFormOpen || poiDrawActive || stopFormOpen}
                style={navItemStyle(stopDrawActive, "#0ea5e9", "#7dd3fc", !!pendingGeometry || heatmapActive || otherToolActive || poiFormOpen || poiDrawActive || stopFormOpen)}
              >
                Durak Ekle
              </button>
            )}
            {canManageTransitRoutes && (
              <button onClick={onRouteManagementToggle} disabled={toolsLocked} style={navItemStyle(routeManagementOpen, "#8b5cf6", "#c4b5fd", toolsLocked)}>
                Güzergah Yönetimi
              </button>
            )}
            <button onClick={onTripPlanningToggle} disabled={toolsLocked} style={navItemStyle(tripPlanningOpen, "#22c55e", "#86efac", toolsLocked)}>
              Yol Tarifi
            </button>
          </NavDropdown>
        )}

        <NavDropdown
          label="Görünüm"
          disabled={toolsLocked}
          hasActiveItem={queryPanelOpen || poiDrawActive || teamModeActive || layerControlOpen}
          onOpenChange={(open) => handleDropdownOpenChange("Görünüm", open)}
        >
          <button onClick={onQueryPanelToggle} disabled={toolsLocked} style={navItemStyle(queryPanelOpen, "#6366f1", "#a5b4fc", toolsLocked)}>
            Geçmiş
          </button>
          <button onClick={onLayerControlToggle} disabled={toolsLocked} style={navItemStyle(layerControlOpen, "#3b82f6", "#93c5fd", toolsLocked)}>
            Filtre
          </button>
          {canManagePoi && (
            <button
              onClick={handlePoiButtonClick}
              disabled={!!pendingGeometry || heatmapActive || otherToolActive || poiFormOpen || stopDrawActive || stopFormOpen}
              style={navItemStyle(poiDrawActive, "#0d9488", "#5eead4", !!pendingGeometry || heatmapActive || otherToolActive || poiFormOpen || stopDrawActive || stopFormOpen)}
            >
              POI
            </button>
          )}
          {canDraw && !isAdmin && teamId !== null && (
            <button onClick={() => setTeamModeActive((p) => !p)} disabled={toolsLocked} style={navItemStyle(teamModeActive, "#10b981", "#6ee7b7", toolsLocked)}>
              Takım Haritası
            </button>
          )}
        </NavDropdown>

        <div style={{ width: 1, height: 24, background: "rgba(255,255,255,.08)" }} />

        <button
          onClick={handleLogout}
          style={{
            padding: "6px 14px", borderRadius: 8,
            border: "1px solid rgba(255,255,255,.15)",
            background: "transparent", color: "#94a3b8",
            fontSize: 13, fontWeight: 500,
            cursor: "pointer", minWidth: 80,
          }}
        >
          Çıkış Yap
        </button>
        </div>
      </nav>

      <BottomToolbar
        activeType={activeType}
        onSelect={handleSelect}
        onCancel={() => { onActiveTypeChange(null); setToast(null); }}
        disabled={toolsLocked}
      />

      {pendingGeometry && (
        <AttributeModal
          pending={pendingGeometry}
          onSave={handleModalSave}
          onCancel={handleModalCancel}
          isSaving={isSaving}
        />
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </>
  );
}

function navItemStyle(active: boolean, activeBorder: string, activeText: string, disabled: boolean): React.CSSProperties {
  return {
    padding: "6px 14px", borderRadius: 8, border: "1px solid",
    borderColor: active ? activeBorder : "rgba(255,255,255,.15)",
    background:  active ? `${activeBorder}33` : "transparent",
    color:       active ? activeText : "#94a3b8",
    fontSize: 13, fontWeight: 500,
    cursor:   disabled ? "not-allowed" : "pointer",
    opacity:  disabled ? 0.5 : 1,
    transition: "all .15s",
    width: "100%", textAlign: "left",
  };
}