import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth }    from "../../../auth/context/AuthContext";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { buildStyle } from "../../../../utils/mapStyle";
import { useDrawing, type DrawingLayers } from "../hooks/useDrawing";
import { useFeatureLoader } from "../hooks/useFeatureLoader";
import { useAnalysis } from "../hooks/useAnalysis";
import { AttributeModal } from "./AttributeModal";
import { Toast }      from "../../../../shared/components/Toast";
import type { DrawType, PendingGeometry } from "../types";
import { createGeoFeature, validateGeometry } from "../api/geoService";
import { useEdgeVisibility } from "../hooks/useEdgeVisibility";
import { BottomToolbar } from "./BottomToolbar";
import { NavDropdown } from "./NavDropdown";

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

  const { logout, apiFetch, roles, teamId } = useAuth();
  const navigate = useNavigate();

  const isPlainUser = roles.includes("User") && roles.length === 1;

  const canManagePoi  =
    roles.includes("Admin") || roles.includes("Takım Lideri") ||
    roles.includes("Çalışan") || roles.includes("POI Operatörü");

  const canManageTransitStops =
    roles.includes("Admin") || roles.includes("Takım Lideri") ||
    roles.includes("Çalışan") || roles.includes("Ulaşım Operatörü");

  const canManageTransitRoutes =
    roles.includes("Admin") || roles.includes("Takım Lideri") || roles.includes("Ulaşım Operatörü");

  const canUseAnalysisTools =
    roles.includes("Admin") || roles.includes("Takım Lideri") || roles.includes("Çalışan");

  const isOperatorOnly =
    (roles.includes("POI Operatörü") || roles.includes("Ulaşım Operatörü")) &&
    !roles.includes("Admin") && !roles.includes("Takım Lideri") && !roles.includes("Çalışan");
  
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
  const isAdmin = roles.includes("Admin");

  const keepBottomVisible = !isPlainUser && (!!pendingGeometry || !!activeType);
  const keepTopVisible = !isPlainUser && (
    analysisActive || heatmapActive || poiDrawActive || poiFormOpen ||
    locAnalysisActive || queryPanelOpen || layerControlOpen || openDropdowns.size > 0
  );

  const topVisible = useEdgeVisibility("top", keepTopVisible);

  if (isPlainUser) {
    return (
      <nav style={{
        position:     "fixed",
        top:          topVisible ? 12 : -80,
        left:         "50%",
        transform:    "translateX(-50%)",
        display:      "flex",
        alignItems:   "center",
        gap:          16,
        background:   "#030c21",
        border:       "1px solid rgba(255,255,255,.08)",
        borderRadius: 12,
        padding:      "8px 16px",
        zIndex:       1000,
        transition:   "top .25s ease",
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            AtaGIS
          </span>
        </div>

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
          Tematik Harita
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
      </nav>
    );
  }

  return (
    <>
      <nav style={{
        position:     "fixed",
        top:          topVisible ? 12 : -80,
        left:         "50%",
        transform:    "translateX(-50%)",
        display:      "flex",
        alignItems:   "center",
        gap:          10,
        background:   "#030c21",
        border:       "1px solid rgba(255,255,255,.08)",
        borderRadius: 12,
        padding:      "8px 16px",
        zIndex:       1000,
        transition:   "top .25s ease",
      }}>

        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#3b82f6" }} />
          <span style={{ color: "#f1f5f9", fontWeight: 600, fontSize: 14, letterSpacing: ".3px" }}>
            AtaGIS
          </span>
        </div>

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
          hasActiveItem={analysisActive || locAnalysisActive || heatmapActive || layerControlOpen || thematicMapOpen}
          onOpenChange={(open) => handleDropdownOpenChange("Analiz", open)}
        >
          {canUseAnalysisTools && (
            <button onClick={handleAnalysisToggle} disabled={toolsLocked} style={navItemStyle(analysisActive, "#eab308", "#fde047", toolsLocked)}>
              Alan Tara
            </button>
          )}
          {canUseAnalysisTools && (
            <button onClick={onLocAnalysisToggle} disabled={toolsLocked} style={navItemStyle(locAnalysisActive, "#ec4899", "#fbcfe8", toolsLocked)}>
              Konum Analizi
            </button>
          )}
          <button onClick={onLayerControlToggle} disabled={toolsLocked} style={navItemStyle(layerControlOpen, "#3b82f6", "#93c5fd", toolsLocked)}>
            Filtre
          </button>
          {canUseAnalysisTools && (
            <button
              onClick={onHeatmapToggle}
              disabled={!!pendingGeometry || poiDrawActive || poiFormOpen}
              style={navItemStyle(heatmapActive, "#ef4444", "#fca5a5", !!pendingGeometry || poiDrawActive || poiFormOpen)}
            >
              Isı Haritası
            </button>
          )}
          <button onClick={onThematicMapToggle} disabled={toolsLocked} style={navItemStyle(thematicMapOpen, "#f59e0b", "#fcd34d", toolsLocked)}>
            Tematik Harita
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
            label="Toplu Taşıma"
            disabled={toolsLocked}
            hasActiveItem={stopDrawActive || routeManagementOpen || tripPlanningOpen}
            onOpenChange={(open) => handleDropdownOpenChange("Toplu Taşıma", open)}
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
          hasActiveItem={queryPanelOpen || poiDrawActive || teamModeActive}
          onOpenChange={(open) => handleDropdownOpenChange("Görünüm", open)}
        >
          <button onClick={onQueryPanelToggle} disabled={toolsLocked} style={navItemStyle(queryPanelOpen, "#6366f1", "#a5b4fc", toolsLocked)}>
            Geçmiş
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
          {!isOperatorOnly && !isAdmin && teamId !== null && (
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
      </nav>

      <BottomToolbar
        activeType={activeType}
        onSelect={handleSelect}
        onCancel={() => { onActiveTypeChange(null); setToast(null); }}
        disabled={toolsLocked}
        keepVisible={keepBottomVisible}
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