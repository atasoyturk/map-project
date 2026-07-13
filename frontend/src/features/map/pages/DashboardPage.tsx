import { useState, useEffect, useRef }     from "react";
import Map                         from "ol/Map";
import VectorSource                from "ol/source/Vector";
import VectorLayer                 from "ol/layer/Vector";
import TileLayer   from "ol/layer/Tile";
import TileWMS     from "ol/source/TileWMS";
import { useAuth } from "../../../features/auth/context/AuthContext";
import { MapView }                 from "../components/MapView";
import { Navbar }                  from "../components/Navbar";
import { InfoPopup }               from "../components/InfoPopup";
import { LayerControl }            from "../components/LayerControl";
import { FeaturePickerModal }      from "../components/FeaturePickerModal";
import { QueryPanel }              from "../components/QueryPanel";
import { FeatureTooltip }          from "../components/FeatureTooltip";
import { AnnotationContextMenu }   from "../components/AnnotationContextMenu";
import { AnnotationModal }         from "../components/AnnotationModal";
import { useMapClick }             from "../hooks/useMapClick";
import { useUserLookup }           from "../hooks/useUserLookup";
import { useFeatureTooltip }       from "../hooks/useFeatureTooltip";
import { useAnnotationContextMenu }from "../hooks/useAnnotationContextMenu";
import { useAnnotationLoader, annotationToFeature } from "../hooks/useAnnotationLoader";
import type { SelectedFeatureInfo } from "../hooks/useSelect";
import type { DrawingLayers }       from "../hooks/useDrawing";
import type { AnnotationResponseDto } from "../../../shared/types/annotation";
import { buildStyle }              from "../../../utils/mapStyle";
import type { DrawType }           from "../../../shared/types/drawing";
import { HeatmapLegend } from "../components/HeatmapLegend";
import { Style, Fill, Stroke, Circle as CircleStyle } from "ol/style";


const annotationStyle = new Style({
  image: new CircleStyle({
    radius: 7,
    fill:   new Fill({ color: "#f59e0b" }),
    stroke: new Stroke({ color: "#78350f", width: 2 }),
  }),
});

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
  const { token, apiFetch } = useAuth();

  const annotationSourceRef = useRef(new VectorSource());
  const annotationLayerRef  = useRef<VectorLayer<VectorSource> | null>(null);

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({
      source: annotationSourceRef.current,
      style:  annotationStyle,
      zIndex: 2,
    });
    map.addLayer(layer);
    annotationLayerRef.current = layer;
    return () => {
      map.removeLayer(layer);
      annotationLayerRef.current = null;
    };
  }, [map]);

  useAnnotationLoader(map, annotationSourceRef.current, apiFetch);

  const interactionsIdle = !analysisActive && activeType === null && !heatmapActive;

  const userLookup = useUserLookup(apiFetch);
  const tooltip = useFeatureTooltip({
    map,
    layers,
    userLookup,
    enabled: interactionsIdle,
  });

  const { pending: pendingNote, clear: clearPendingNote } = useAnnotationContextMenu({
    map,
    enabled: interactionsIdle,
  });

  useMapClick({
    map,
    enabled: !analysisActive && activeType === null,
    onFeaturesFound: (found) => {
      if (found.length === 0) { setSelected(null); setCandidates([]); return; }
      if (found.length === 1) { setSelected(found[0]); setCandidates([]); return; }
      setCandidates(found);
    },
  });

  useEffect(() => {
    if (!map) return;

    // hide point layer
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
      // reopen point layer
      if (layers?.pointLayer) {
        layers.pointLayer.setVisible(true);
      }
    };
  }, [map, heatmapActive, token, layers]);

  // feature choice effect
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
      const res = await apiFetch("/api/annotation", {
        method: "POST",
        body:   JSON.stringify({ noteText, wktGeometry: pendingNote.wkt }),
      });

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

  function handleCancelNote() {
    setShowNoteModal(false);
    setNoteError(null);
    clearPendingNote();
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
      />
      <div style={{ position: "relative", marginTop: 50, flex: 1 }}>
        <MapView onMapReady={setMap} height="calc(100vh - 56px)" />
        <HeatmapLegend visible={heatmapActive} />
        <LayerControl layers={layers} visible={layerControlOpen} />
        <FeatureTooltip info={tooltip} />

        {pendingNote && !showNoteModal && (
          <AnnotationContextMenu
            x={pendingNote.x}
            y={pendingNote.y}
            onAddNote={() => setShowNoteModal(true)}
            onClose={clearPendingNote}
          />
        )}
      </div>

      {showNoteModal && (
        <AnnotationModal
          onSave={handleSaveNote}
          onCancel={handleCancelNote}
          isSaving={isSavingNote}
          error={noteError}
        />
      )}

      {selected && (
        <InfoPopup
          info={selected}
          onClose={() => setSelected(null)}
          onUpdated={(name, color) => {
            selected.feature.setStyle(buildStyle(color, name));
            setSelected(null);
          }}
          onDelete={() => {
            (selected.feature.get("source") as VectorSource)?.removeFeature(selected.feature);
            setSelected(null);
          }}
        />
      )}

      {candidates.length > 1 && (
        <FeaturePickerModal
          features={candidates}
          onPick={(info) => { setSelected(info); setCandidates([]); }}
          onClose={() => setCandidates([])}
        />
      )}

      {queryPanelOpen && (
        <QueryPanel
          map={map}
          onClose={() => setQueryPanelOpen(false)}
        />
      )}
    </div>
  );
}