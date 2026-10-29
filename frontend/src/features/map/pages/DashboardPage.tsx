import { useState, useEffect, useRef }     from "react";
import Map                         from "ol/Map";
import Feature                      from "ol/Feature";

import VectorSource                from "ol/source/Vector";
import Cluster                     from "ol/source/Cluster";
import VectorLayer                 from "ol/layer/Vector";
import Point from "ol/geom/Point";
import { fromLonLat } from "ol/proj";

import { useAuth }                from "../../../features/auth/context/AuthContext";
import { MapView }              from "../core/components/MapView";
import { Navbar }               from "../core/components/Navbar";
import { InfoPopup }            from "../core/components/InfoPopup";
import { LayerControl }         from "../core/components/LayerControl";
import { FeaturePickerModal }   from "../core/components/FeaturePickerModal";
import { QueryPanel }           from "../core/components/QueryPanel";
import { FeatureTooltip }       from "../core/components/FeatureTooltip";
import { HeatmapLegend }        from "../core/components/HeatmapLegend";
import { AnnotationContextMenu} from "../annotation/components/AnnotationContextMenu";
import { AnnotationModal }      from "../annotation/components/AnnotationModal";
import { useAnnotationClick } from "../annotation/hooks/useAnnotationClick";
import { AnnotationInfoPopup } from "../annotation/components/AnnotationInfoPopup";
import { createAnnotation, deleteAnnotation } from "../annotation/api/annotationService";

import { PoiModule }            from "../poi/components/PoiModule";
import { PoiSearchBar }         from "../poi/components/PoiSearchBar";
import { TransitModule }        from "../transit/components/TransitModule";

import { useMapClick }          from "../core/hooks/useMapClick";
import { useUserLookup }        from "../core/hooks/useUserLookup";
import { useAnalysisHeatmapLayer } from "../core/hooks/useAnalysisHeatmapLayer";
import { useSearchMarkerLayer }   from "../core/hooks/useSearchMarkerLayer";
import { useWmsHeatmapLayer }     from "../core/hooks/useWmsHeatmapLayer";
import { useRegionPreviewLayer }  from "../core/hooks/useRegionPreviewLayer";
import { useAnnotationLayer }     from "../annotation/hooks/useAnnotationLayer";
import { useFeatureTooltip }    from "../core/hooks/useFeatureTooltip";
import type { SelectedFeatureInfo } from "../core/hooks/useSelect";
import type { DrawingLayers }       from "../core/hooks/useDrawing";
import { useAnnotationContextMenu } from "../annotation/hooks/useAnnotationContextMenu";
import { useAnnotationLoader, annotationToFeature } from "../annotation/hooks/useAnnotationLoader";
import { useCategoryTree }      from "../poi/hooks/useCategoryTree";
import { useOsmImport }         from "../poi/hooks/useOsmImport";
import { LocationAnalysisPanel } from "../core/components/LocationAnalysisPanel";
import { getLocationAnalysis } from "../core/api/analysisService";

import { Toast, type ToastState }   from "../../../shared/components/Toast";
import { Z }                        from "../../../shared/components/ui/zIndex";
import { LocationSearchBar }        from "../core/components/LocationSearchBar";
import { buildStyle }               from "../core/mapStyle";
import type { DrawType }            from "../core/types";
import type { AnnotationResponseDto} from "../annotation/types";

import { TripPlanningPanel } from "../core/components/TripPlanningPanel";
import { useTripPointClick } from "../core/hooks/useTripPointClick";
import { useTripRouteLayer } from "../core/hooks/useTripRouteLayer";
import type { TripPlanResultDto } from "../core/api/tripPlanningService";

import { ThematicMapPanel } from "../core/components/ThematicMapPanel";
import { LocationReportPanel } from "../core/components/LocationReportPanel";
import { useIsochroneLayer } from "../core/hooks/useIsochroneLayer";
import type { IsochroneResultDto } from "../core/api/thematicMapService";

export function DashboardPage() {
  const [map,             setMap]            = useState<Map | null>(null);
  const [selected,        setSelected]       = useState<SelectedFeatureInfo | null>(null);
  const [candidates,      setCandidates]     = useState<SelectedFeatureInfo[]>([]);
  const [analysisActive,  setAnalysisActive] = useState(false);
  const [activeType,      setActiveType]     = useState<DrawType | null>(null);
  const [layers,          setLayers]         = useState<DrawingLayers | null>(null);
  const [queryPanelOpen,  setQueryPanelOpen] = useState(false);
  const [layerControlOpen,setLayerControlOpen] = useState(false);
  const [heatmapActive, setHeatmapActive] = useState(false);
  const [isSavingNote,  setIsSavingNote]  = useState(false);
  const [noteError,     setNoteError]     = useState<string | null>(null);
  const [showNoteModal, setShowNoteModal] = useState(false);
  const [toast,         setToast]         = useState<ToastState | null>(null);

  const [poiDrawActive, setPoiDrawActive] = useState(false);
  const [poiFormOpen,   setPoiFormOpen]   = useState(false);
  const [poiFeatures,   setPoiFeatures]   = useState<Feature[]>([]);
  const [poiLayer,      setPoiLayer]      = useState<VectorLayer<VectorSource> | null>(null);
  const [poiReloadKey,  setPoiReloadKey]  = useState(0);

  const [stopDrawActive,      setStopDrawActive]      = useState(false);
  const [stopFormOpen,        setStopFormOpen]        = useState(false);
  const [routeManagementOpen, setRouteManagementOpen] = useState(false);

  const [tripPlanningOpen, setTripPlanningOpen] = useState(false);
  const [tripPickMode,     setTripPickMode]     = useState<"start" | "end" | null>(null);
  const [tripSearchTarget, setTripSearchTarget] = useState<"start" | "end" | null>(null);
  const [tripStartPoint,   setTripStartPoint]   = useState<{ lon: number; lat: number } | null>(null);
  const [tripEndPoint,     setTripEndPoint]     = useState<{ lon: number; lat: number } | null>(null);
  const [tripResult,       setTripResult]       = useState<TripPlanResultDto | null>(null);

  const [thematicMapOpen,  setThematicMapOpen]  = useState(false);
  const [thematicPickMode, setThematicPickMode] = useState<"center" | null>(null);
  const [thematicSearchTarget, setThematicSearchTarget] = useState<"center" | null>(null);
  const [thematicCenterPoint,  setThematicCenterPoint]  = useState<{ lon: number; lat: number } | null>(null);
  const [thematicResult,       setThematicResult]       = useState<IsochroneResultDto | null>(null);

  const [reportOpen,         setReportOpen]         = useState(false);
  const [reportPickMode,     setReportPickMode]     = useState<"center" | null>(null);
  const [reportSearchTarget, setReportSearchTarget] = useState<"center" | null>(null);
  const [reportCenter,       setReportCenter]       = useState<{ lon: number; lat: number; address?: string } | null>(null);
  const [reportResult,       setReportResult]       = useState<IsochroneResultDto | null>(null);

  const { token, apiFetch, permissions, hasPermission, userId } = useAuth();

  const isPlainUser = !(hasPermission("point_create") || hasPermission("line_create") || hasPermission("polygon_create"));
  const canReadAnnotation = hasPermission("annotation_read");

  const annotationSourceRef = useRef(new VectorSource());
  const searchMarkerSourceRef = useRef(new VectorSource());

  const categories = useCategoryTree(apiFetch);
  const osmImport  = useOsmImport(map, apiFetch, setToast, () => setPoiReloadKey((k) => k + 1));
  const [locAnalysisPanelOpen, setLocAnalysisPanelOpen] = useState(false);
  const [locAnalysisPolygon, setLocAnalysisPolygon]    = useState<{ wkt: string; feature: any } | null>(null);
  const [locAnalysisPoints, setLocAnalysisPoints] = useState<{latitude: number, longitude: number, weight: number}[] | null>(null);
  const regionPreview   = useRegionPreviewLayer(map);
  const annotationLayer = useAnnotationLayer(map, annotationSourceRef.current, canReadAnnotation);

  useAnalysisHeatmapLayer(map, locAnalysisPoints);
  useSearchMarkerLayer(map, searchMarkerSourceRef.current);

  function handleLocationSelected(lon: number, lat: number, address: string) {
    searchMarkerSourceRef.current.clear();
    searchMarkerSourceRef.current.addFeature(
      new Feature({ geometry: new Point(fromLonLat([lon, lat])) })
    );

    if (tripSearchTarget === "start") { setTripStartPoint({ lon, lat }); setTripSearchTarget(null); }
    else if (tripSearchTarget === "end") { setTripEndPoint({ lon, lat }); setTripSearchTarget(null); }
    else if (thematicSearchTarget === "center") { setThematicCenterPoint({ lon, lat }); setThematicSearchTarget(null); }
    else if (reportSearchTarget === "center") { setReportCenter({ lon, lat, address }); setReportSearchTarget(null); }
  }

  // Annotations are loaded only for users with annotation_read (internal notes).
  useAnnotationLoader(map, annotationSourceRef.current, apiFetch, canReadAnnotation);

  const poiFlowActive    = poiDrawActive || poiFormOpen;
  const stopFlowActive   = stopDrawActive || stopFormOpen;
  const interactionsIdle = !analysisActive && activeType === null && !heatmapActive && !poiFlowActive && !stopFlowActive;

  const userLookup = useUserLookup(apiFetch);
  const tooltip = useFeatureTooltip({
    map,
    layers,
    userLookup,
    enabled: interactionsIdle && !isPlainUser,
  });

  const canCreateAnnotation = hasPermission("annotation_create");

  const { pending: pendingNote, clear: clearPendingNote } = useAnnotationContextMenu({
    map,
    enabled: interactionsIdle && !isPlainUser && canCreateAnnotation,
  });

  const { selected: annotationSelected, clear: clearAnnotationSelected } = useAnnotationClick({
    map,
    annotationLayer: annotationLayer,
    enabled: interactionsIdle && canReadAnnotation,
  });

  useMapClick({
    map,
    enabled: !isPlainUser && !analysisActive && activeType === null && !poiFlowActive && !stopFlowActive,
    onFeaturesFound: (found) => {
      if (found.length === 0) { setSelected(null); setCandidates([]); return; }
      if (found.length === 1) { setSelected(found[0]); setCandidates([]); return; }
      setCandidates(found);
    },
  });

  useTripPointClick({
    map,
    enabled: tripPickMode !== null,
    onPick: (lon, lat) => {
      if (tripPickMode === "start") setTripStartPoint({ lon, lat });
      else if (tripPickMode === "end") setTripEndPoint({ lon, lat });
      setTripPickMode(null);
    },
  });

  useTripRouteLayer(map, tripResult, tripStartPoint, tripEndPoint);

  useTripPointClick({
    map,
    enabled: thematicPickMode !== null,
    onPick: (lon, lat) => {
      setThematicCenterPoint({ lon, lat });
      setThematicPickMode(null);
    },
  });

  useIsochroneLayer(map, thematicResult);

  useTripPointClick({
    map,
    enabled: reportPickMode !== null,
    onPick: (lon, lat) => {
      setReportCenter({ lon, lat });
      setReportPickMode(null);
    },
  });

  useIsochroneLayer(map, reportResult);

  useWmsHeatmapLayer(map, heatmapActive, token, layers, !isPlainUser);

  useEffect(() => {
    function handleSelectFeature(e: Event) {
      const info = (e as CustomEvent<SelectedFeatureInfo>).detail;
      setSelected(info);
    }
    window.addEventListener("gis:selectFeature", handleSelectFeature);
    return () => window.removeEventListener("gis:selectFeature", handleSelectFeature);
  }, []);

  async function handleSaveNote(noteText: string) {
    if (!pendingNote) return;
    setIsSavingNote(true);
    setNoteError(null);

    try {
      const res = await createAnnotation(apiFetch, { noteText, wktGeometry: pendingNote.wkt });

      if (!res.ok) {
        setNoteError(res.status === 403 ? "Not ekleme yetkiniz bulunmuyor." : "Not kaydedilemedi.");
        return;
      }

      const dto: AnnotationResponseDto = await res.json();
      annotationSourceRef.current.addFeature(annotationToFeature(dto));
      setShowNoteModal(false);
      clearPendingNote();
    } catch {
      setNoteError("Sunucuya bağlanılamadı.");
    } finally {
      setIsSavingNote(false);
    }
  }

  async function handleDeleteAnnotation(id: number) {
    try {
      const res = await deleteAnnotation(apiFetch, id);

      if (!res.ok) {
        const message = res.status === 403 ? "Bu notu silme yetkiniz bulunmuyor." : "Not silinemedi.";
        setToast({ message, type: "error" });
        return;
      }

      if (annotationSelected) {
        annotationSourceRef.current.removeFeature(annotationSelected.feature);
      }

      clearAnnotationSelected();
      setToast({ message: "Not başarıyla silindi.", type: "success" });
    } catch {
      setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    }
  }

  function handleCancelNote() {
    setShowNoteModal(false);
    setNoteError(null);
    clearPendingNote();
  }

  async function handleStartLocationAnalysis(criteria: { categoryId: number; score: number }[]) {
    if (!locAnalysisPolygon) return;

    try {
      const res = await getLocationAnalysis(apiFetch, {
        wktGeometry: locAnalysisPolygon.wkt,
        criteria: criteria
      });

      if (!res.ok) {
        const msg = await res.text();
        setToast({ message: `Analiz Hatası: ${msg}`, type: "error" });
        return;
      }

      const data = await res.json();
      setLocAnalysisPoints(data.heatmapPoints);

      setToast({ message: "Konum analizi tamamlandı, ısı haritası oluşturuldu.", type: "success" });
    } catch {
      setToast({ message: "Sunucuya bağlanılamadı.", type: "error" });
    }
  }

  function getFeatureSource(type: string): VectorSource | null {
    if (!layers) return null;
    if (type === "point")   return (layers.pointLayer.getSource() as Cluster)?.getSource() ?? null;
    if (type === "line")    return layers.lineLayer.getSource();
    if (type === "polygon") return layers.polygonLayer.getSource();
    return null;
  }

  function handleClearLocationAnalysis() {
    regionPreview.clear();

    if (locAnalysisPolygon?.feature) {
      map?.getLayers().getArray().forEach(layer => {
        if (layer instanceof VectorLayer) {
          const source = layer.getSource();
          if (source && source.hasFeature(locAnalysisPolygon.feature)) {
            source.removeFeature(locAnalysisPolygon.feature);
          }
        }
      });
    }

    setLocAnalysisPoints(null);
    setLocAnalysisPolygon(null);
    setToast({ message: "Konum analizi sonuçları temizlendi.", type: "success" });
  }

  return (
    <div style={{ display: "flex", flexDirection: "column", height: "100vh" }}>
      <Navbar
        map={map}
        analysisActive={analysisActive}
        onAnalysisChange={setAnalysisActive}
        activeType={activeType}
        onActiveTypeChange={setActiveType}
        onLayersReady={setLayers}
        queryPanelOpen={queryPanelOpen}
        onQueryPanelToggle={() => setQueryPanelOpen((p) => !p)}
        layerControlOpen={layerControlOpen}
        onLayerControlToggle={() => setLayerControlOpen((p) => !p)}
        heatmapActive={heatmapActive}
        onHeatmapToggle={() => setHeatmapActive((p) => !p)}
        poiDrawActive={poiDrawActive}
        onPoiDrawChange={setPoiDrawActive}
        poiFormOpen={poiFormOpen}
        locAnalysisActive={locAnalysisPanelOpen}
        onLocAnalysisToggle={() => setLocAnalysisPanelOpen(!locAnalysisPanelOpen)}
        onLocAnalysisPolygonReady={(wkt, feature: any) => {
          setLocAnalysisPolygon({ wkt, feature });
          setToast({ message: "Bölge başarıyla seçildi. Şimdi kriterlerinizi belirleyebilirsiniz.", type: "success" });
        }}
        stopDrawActive={stopDrawActive}
        onStopDrawChange={setStopDrawActive}
        stopFormOpen={stopFormOpen}
        routeManagementOpen={routeManagementOpen}
        onRouteManagementToggle={() => setRouteManagementOpen((p) => !p)}
        tripPlanningOpen={tripPlanningOpen}
        onTripPlanningToggle={() => setTripPlanningOpen((p) => !p)}
        thematicMapOpen={thematicMapOpen}
        onThematicMapToggle={() => setThematicMapOpen((p) => !p)}
        locationReportOpen={reportOpen}
        onLocationReportToggle={() => setReportOpen((p) => !p)}
        osmImportBusy={osmImport.busy}
        onOsmImport={osmImport.run}
      />
      <div style={{ position: "relative", flex: 1 }}>
        <MapView onMapReady={setMap} height="100vh" />

        {osmImport.busy && (
          <div
            className="absolute top-20 left-1/2 -translate-x-1/2 rounded-lg border border-white/10 bg-[#030c21] px-4 py-2 text-[13px] text-slate-300 shadow-lg"
            style={{ zIndex: Z.toolbar }}
          >
            OSM verisi alınıyor…
          </div>
        )}

        {annotationSelected && (
          <div style={{ position: "absolute", left: annotationSelected.x, top: annotationSelected.y }}>
            <AnnotationInfoPopup
              feature={annotationSelected.feature}
              userLookup={userLookup}
              onClose={clearAnnotationSelected}
              onDelete={() => handleDeleteAnnotation(annotationSelected.feature.get("id"))}
              canDelete={hasPermission("admin_access") || annotationSelected.feature.get("userId") === userId}
            />
          </div>
        )}

        {locAnalysisPanelOpen && (
          <LocationAnalysisPanel
            categories={categories}
            onStart={handleStartLocationAnalysis}
            onCancel={() => {
              regionPreview.clear();
              setLocAnalysisPanelOpen(false);
              setLocAnalysisPolygon(null);
            }}
            isPolygonSelected={!!locAnalysisPolygon}
            onSelectPolygon={() => {
              setActiveType("Polygon");
              setToast({ message: "Analiz için hedef bölgeyi harita üzerinde çizin.", type: "success" });
            }}
            onClear={handleClearLocationAnalysis}
            hasResults={!!locAnalysisPoints}
            onRegionSelect={(wkt, displayName) => {
              regionPreview.show(wkt);

              setLocAnalysisPolygon({ wkt, feature: null });
              setToast({ message: `"${displayName}" seçildi.`, type: "success" });
            }}
          />
        )}

        {hasPermission("heatmap_view") && <HeatmapLegend visible={heatmapActive} />}
        {!isPlainUser && <LayerControl layers={layers} poiLayer={poiLayer} visible={layerControlOpen} />}
        <PoiSearchBar map={map} poiFeatures={poiFeatures} />
        <LocationSearchBar
          map={map}
          onLocationSelected={handleLocationSelected}
          active={tripSearchTarget !== null || thematicSearchTarget !== null || reportSearchTarget !== null}
          onQueryCleared={() => searchMarkerSourceRef.current.clear()}
        />
        {!isPlainUser && <FeatureTooltip info={tooltip} />}

        {!isPlainUser && pendingNote && !showNoteModal && (
          <AnnotationContextMenu
            x={pendingNote.x}
            y={pendingNote.y}
            onAddNote={() => setShowNoteModal(true)}
            onClose={clearPendingNote}
          />
        )}
      </div>

      {!isPlainUser && showNoteModal && (
        <AnnotationModal
          onSave={handleSaveNote}
          onCancel={handleCancelNote}
          isSaving={isSavingNote}
          error={noteError}
        />
      )}

      <PoiModule
        map={map}
        apiFetch={apiFetch}
        categories={categories}
        userLookup={userLookup}
        permissions={permissions}
        userId={userId}
        canRead={hasPermission("poi_read")}
        otherFlowsIdle={!analysisActive && activeType === null && !heatmapActive && !stopFlowActive}
        drawActive={poiDrawActive}
        onDrawActiveChange={setPoiDrawActive}
        onFormOpenChange={setPoiFormOpen}
        onFeaturesChange={setPoiFeatures}
        onLayerReady={setPoiLayer}
        onToast={setToast}
        reloadKey={poiReloadKey}
      />

      <TransitModule
        map={map}
        apiFetch={apiFetch}
        userLookup={userLookup}
        permissions={permissions}
        userId={userId}
        canRead={hasPermission("transit_stop_read")}
        otherFlowsIdle={!analysisActive && activeType === null && !heatmapActive && !poiFlowActive}
        drawActive={stopDrawActive}
        onDrawActiveChange={setStopDrawActive}
        onFormOpenChange={setStopFormOpen}
        onToast={setToast}
        routeManagementOpen={routeManagementOpen}
        onRouteManagementClose={() => setRouteManagementOpen(false)}
      />

      {tripPlanningOpen && (
        <TripPlanningPanel
          onClose={() => {
            setTripPlanningOpen(false);
            setTripPickMode(null);
            setTripSearchTarget(null);
            setTripStartPoint(null);
            setTripEndPoint(null);
            setTripResult(null);
          }}
          pickMode={tripPickMode}
          onPickModeChange={setTripPickMode}
          searchTarget={tripSearchTarget}
          onSearchTargetChange={setTripSearchTarget}
          startPoint={tripStartPoint}
          endPoint={tripEndPoint}
          onResult={setTripResult}
        />
      )}

      {thematicMapOpen && (
        <ThematicMapPanel
          onClose={() => {
            setThematicMapOpen(false);
            setThematicPickMode(null);
            setThematicSearchTarget(null);
            setThematicCenterPoint(null);
            setThematicResult(null);
          }}
          pickMode={thematicPickMode}
          onPickModeChange={setThematicPickMode}
          searchTarget={thematicSearchTarget}
          onSearchTargetChange={setThematicSearchTarget}
          centerPoint={thematicCenterPoint}
          onResult={setThematicResult}
        />
      )}

      {reportOpen && (
        <LocationReportPanel
          onClose={() => {
            setReportOpen(false);
            setReportPickMode(null);
            setReportSearchTarget(null);
            setReportCenter(null);
            setReportResult(null);
          }}
          categories={categories}
          pickMode={reportPickMode}
          onPickModeChange={setReportPickMode}
          searchTarget={reportSearchTarget}
          onSearchTargetChange={setReportSearchTarget}
          center={reportCenter}
          onResult={setReportResult}
        />
      )}

      {!isPlainUser && selected && (
        <InfoPopup
          info={selected}
          onClose={() => setSelected(null)}
          onUpdated={(name, color) => {
            selected.feature.setStyle(buildStyle(color, name));
            setSelected(null);
          }}
          onDelete={() => {
            const source = getFeatureSource(selected.type);
            if (source) source.removeFeature(selected.feature);
            setSelected(null);
            setCandidates([]);
          }}
        />
      )}

      {!isPlainUser && candidates.length > 1 && (
        <FeaturePickerModal
          features={candidates}
          onPick={(info) => { setSelected(info); setCandidates([]); }}
          onClose={() => setCandidates([])}
        />
      )}

      {!isPlainUser && queryPanelOpen && (
        <QueryPanel
          map={map}
          onClose={() => setQueryPanelOpen(false)}
        />
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  );
}