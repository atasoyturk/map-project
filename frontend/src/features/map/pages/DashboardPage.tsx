import { useState, useEffect, useRef }     from "react";
import Map                         from "ol/Map";
import Feature                      from "ol/Feature";

import VectorSource                from "ol/source/Vector";
import Cluster                     from "ol/source/Cluster";
import VectorLayer                 from "ol/layer/Vector";
import HeatmapLayer                from "ol/layer/Heatmap";
import Point from "ol/geom/Point";
import { fromLonLat } from "ol/proj";

import TileLayer                   from "ol/layer/Tile";
import TileWMS                     from "ol/source/TileWMS";
import { useAuth }                 from "../../../features/auth/context/AuthContext";
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
import { useFeatureTooltip }    from "../core/hooks/useFeatureTooltip";
import type { SelectedFeatureInfo } from "../core/hooks/useSelect";
import type { DrawingLayers }       from "../core/hooks/useDrawing";
import { useAnnotationContextMenu } from "../annotation/hooks/useAnnotationContextMenu";
import { useAnnotationLoader, annotationToFeature } from "../annotation/hooks/useAnnotationLoader";
import { useCategoryTree }      from "../poi/hooks/useCategoryTree";
import { LocationAnalysisPanel } from "../core/components/LocationAnalysisPanel";
import { getLocationAnalysis } from "../core/api/analysisService";

import { Toast }                    from "../../../shared/components/Toast";
import { LocationSearchBar }        from "../core/components/LocationSearchBar";
import { buildStyle }               from "../../../utils/mapStyle";
import type { DrawType }            from "../core/types";
import type { AnnotationResponseDto} from "../annotation/types";

import { TripPlanningPanel } from "../core/components/TripPlanningPanel";
import { useTripPointClick } from "../core/hooks/useTripPointClick";
import { useTripRouteLayer } from "../core/hooks/useTripRouteLayer";
import type { TripPlanResultDto } from "../../../shared/api/tripPlanningService";

import { ThematicMapPanel } from "../core/components/ThematicMapPanel";
import { useIsochroneLayer } from "../core/hooks/useIsochroneLayer";
import type { IsochroneResultDto } from "../../../shared/api/thematicMapService";

import { Style, Icon } from "ol/style";
import WKT from "ol/format/WKT";
import { Fill, Stroke} from "ol/style";

const annotationStyle = new Style({
  image: new Icon({
    anchor: [0.5, 1],
    anchorXUnits: "fraction",
    anchorYUnits: "fraction",
    src: 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="%23f59e0b" stroke="%2378350f" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3" fill="white"></circle></svg>',
    scale: 0.5
  } ),
});

interface ToastState {
  message: string;
  type:    "success" | "error";
}

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

  const { token, apiFetch, roles, userId } = useAuth();

  const isPlainUser = roles.includes("User") && roles.length === 1;

  const annotationSourceRef = useRef(new VectorSource());
  const annotationLayerRef  = useRef<VectorLayer<VectorSource> | null>(null);
  const searchMarkerSourceRef = useRef(new VectorSource());

  const categories = useCategoryTree(apiFetch);
  const [locAnalysisPanelOpen, setLocAnalysisPanelOpen] = useState(false);
  const [locAnalysisPolygon, setLocAnalysisPolygon]    = useState<{ wkt: string; feature: any } | null>(null);
  const [locAnalysisPoints, setLocAnalysisPoints] = useState<{latitude: number, longitude: number, weight: number}[] | null>(null);
  const regionPreviewLayerRef = useRef<VectorLayer<VectorSource> | null>(null);
  const [annotationLayer, setAnnotationLayer] = useState<VectorLayer<VectorSource> | null>(null);

  useEffect(() => {
    if (!map || isPlainUser) return;
    const layer = new VectorLayer({
      source: annotationSourceRef.current,
      style:  annotationStyle,
      zIndex: 2,
    });

    map.addLayer(layer);
    annotationLayerRef.current = layer;
    setAnnotationLayer(layer);

    return () => {
      map.removeLayer(layer);
      annotationLayerRef.current = null;
      setAnnotationLayer(null);
    };
  }, [map, isPlainUser]);

  useEffect(() => {
    if (!map || !locAnalysisPoints) return;

    const vectorSource = new VectorSource();

    locAnalysisPoints.forEach(pt => {
      const feature = new Feature({
        geometry: new Point(fromLonLat([pt.longitude, pt.latitude])),
        weight: pt.weight
      });
      vectorSource.addFeature(feature);
    });

    const heatmapLayer = new HeatmapLayer({
      source: vectorSource,
      blur: 20,
      radius: 10,
      weight: (feature: Feature) => feature.get("weight"),
      zIndex: 100
    });

    map.addLayer(heatmapLayer);

    return () => {
      map.removeLayer(heatmapLayer);
    };
  }, [map, locAnalysisPoints]);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({
      source: searchMarkerSourceRef.current,
      style: new Style({
        image: new Icon({
          anchor: [0.5, 1],
          anchorXUnits: "fraction",
          anchorYUnits: "fraction",
          src: 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="%233b82f6" stroke="%231e3a8a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3" fill="white"></circle></svg>',
          scale: 0.5,
        }),
      }),
      zIndex: 200,
    });

    map.addLayer(layer);

    return () => { map.removeLayer(layer); };
  }, [map]);

  function handleLocationSelected(lon: number, lat: number) {
    searchMarkerSourceRef.current.clear();
    searchMarkerSourceRef.current.addFeature(
      new Feature({ geometry: new Point(fromLonLat([lon, lat])) })
    );

    if (tripSearchTarget === "start") { setTripStartPoint({ lon, lat }); setTripSearchTarget(null); }
    else if (tripSearchTarget === "end") { setTripEndPoint({ lon, lat }); setTripSearchTarget(null); }
    else if (thematicSearchTarget === "center") { setThematicCenterPoint({ lon, lat }); setThematicSearchTarget(null); }
  }

  // Annotation'lar User için hiç yüklenmez (şirket dışı kullanıcı, iç notlara erişemez).
  useAnnotationLoader(map, annotationSourceRef.current, apiFetch, !isPlainUser);

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

  const canCreateAnnotation = roles.includes("Admin") || roles.includes("Takım Lideri");

  const { pending: pendingNote, clear: clearPendingNote } = useAnnotationContextMenu({
    map,
    enabled: interactionsIdle && !isPlainUser && canCreateAnnotation,
  });

  const { selected: annotationSelected, clear: clearAnnotationSelected } = useAnnotationClick({
    map,
    annotationLayer: annotationLayer,
    enabled: interactionsIdle && !isPlainUser,
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

  useEffect(() => {
    if (!map || isPlainUser) return;

    if (layers?.pointLayer) {
      layers.pointLayer.setVisible(!heatmapActive);
    }

    if (!heatmapActive) return;

    const heatmapLayer = new TileLayer({
      source: new TileWMS({
        url:    "http://localhost:5130/api/proxy/geoserver/wms",
        params: {
          typeName:    "tbl_point",
          styles:      "gisportal:gisportal_heatmap",
          FORMAT:      "image/png",
          TRANSPARENT: true,
        },
        tileLoadFunction: (tile: any, src: string) => {
          fetch(src, {
            headers: { Authorization: `Bearer ${token}` },
          })
            .then((res) => res.blob())
            .then((blob) => {
              tile.getImage().src = URL.createObjectURL(blob);
            });
        },
      }),
      zIndex:   10,
      opacity:  0.75,
    });

    map.addLayer(heatmapLayer);

    return () => {
      map.removeLayer(heatmapLayer);
      if (layers?.pointLayer) {
        layers.pointLayer.setVisible(true);
      }
    };
  }, [map, heatmapActive, token, layers, isPlainUser]);

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
    if (regionPreviewLayerRef.current && map) {
      map.removeLayer(regionPreviewLayerRef.current);
      regionPreviewLayerRef.current = null;
    }

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
      />
      <div style={{ position: "relative", flex: 1 }}>
        <MapView onMapReady={setMap} height="100vh" />

        {annotationSelected && (
          <div style={{ position: "absolute", left: annotationSelected.x, top: annotationSelected.y }}>
            <AnnotationInfoPopup
              feature={annotationSelected.feature}
              userLookup={userLookup}
              onClose={clearAnnotationSelected}
              onDelete={() => handleDeleteAnnotation(annotationSelected.feature.get("id"))}
              canDelete={roles.includes("Admin") || annotationSelected.feature.get("userId") === userId}
            />
          </div>
        )}

        {locAnalysisPanelOpen && (
          <LocationAnalysisPanel
            categories={categories}
            onStart={handleStartLocationAnalysis}
            onCancel={() => {
              if (regionPreviewLayerRef.current && map) {
                map.removeLayer(regionPreviewLayerRef.current);
                regionPreviewLayerRef.current = null;
              }
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
              if (regionPreviewLayerRef.current && map) {
                map.removeLayer(regionPreviewLayerRef.current);
                regionPreviewLayerRef.current = null;
              }

              const geometry = new WKT().readGeometry(wkt, {
                dataProjection:    "EPSG:4326",
                featureProjection: "EPSG:3857",
              });

              const source = new VectorSource({ features: [new Feature({ geometry })] });
              const layer  = new VectorLayer({
                source,
                style: new Style({
                  fill:   new Fill({ color: "rgba(59,130,246,0.2)" }),
                  stroke: new Stroke({ color: "#3b82f6", width: 2 }),
                }),
                zIndex: 50,
              });

              map?.addLayer(layer);
              regionPreviewLayerRef.current = layer;

              map?.getView().fit(geometry.getExtent(), {
                padding: [60, 60, 60, 60],
                maxZoom: 12,
                duration: 500,
              });

              setLocAnalysisPolygon({ wkt, feature: null });
              setToast({ message: `"${displayName}" seçildi.`, type: "success" });
            }}
          />
        )}

        {!isPlainUser && <HeatmapLegend visible={heatmapActive} />}
        {!isPlainUser && <LayerControl layers={layers} poiLayer={poiLayer} visible={layerControlOpen} />}
        <PoiSearchBar map={map} poiFeatures={poiFeatures} />
        <LocationSearchBar map={map} onLocationSelected={handleLocationSelected} active={tripSearchTarget !== null || thematicSearchTarget !== null} />
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
        roles={roles}
        userId={userId}
        isPlainUser={isPlainUser}
        otherFlowsIdle={!analysisActive && activeType === null && !heatmapActive && !stopFlowActive}
        drawActive={poiDrawActive}
        onDrawActiveChange={setPoiDrawActive}
        onFormOpenChange={setPoiFormOpen}
        onFeaturesChange={setPoiFeatures}
        onLayerReady={setPoiLayer}
        onToast={setToast}
      />

      <TransitModule
        map={map}
        apiFetch={apiFetch}
        userLookup={userLookup}
        roles={roles}
        userId={userId}
        isPlainUser={isPlainUser}
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