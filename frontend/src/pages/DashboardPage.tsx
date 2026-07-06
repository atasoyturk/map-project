import { useState }          from "react";
import Map                   from "ol/Map";
import VectorSource          from "ol/source/Vector";
import { MapView }           from "../components/MapView";
import { Navbar }            from "../components/Navbar";
import { InfoPopup }         from "../components/InfoPopup";
import { LayerControl }      from "../components/LayerControl";
import { useSelect }         from "../hooks/useSelect";
import type { SelectedFeatureInfo } from "../hooks/useSelect";
import type { DrawingLayers }       from "../hooks/useDrawing";
import { buildStyle }        from "../utils/mapStyle";
import type { DrawType }     from "../types/drawing";
import { QueryPanel } from "../components/QueryPanel";


export function DashboardPage() {
  const [map,            setMap]           = useState<Map | null>(null);
  const [selected,       setSelected]      = useState<SelectedFeatureInfo | null>(null);
  const [analysisActive, setAnalysisActive] = useState(false);
  const [activeType,     setActiveType]     = useState<DrawType | null>(null);
  const [layers,         setLayers]         = useState<DrawingLayers | null>(null);
  const [queryPanelOpen, setQueryPanelOpen] = useState(false);

  useSelect({
    map,
    enabled:  !analysisActive && activeType === null,
    onSelect: setSelected,
  });

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
      />
      <div style={{ position: "relative", marginTop: 56, flex: 1 }}>
        <MapView onMapReady={setMap} height="calc(100vh - 56px)" />
        <LayerControl layers={layers} />
      </div>

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
      
      {queryPanelOpen && (    
        <QueryPanel onClose={() => setQueryPanelOpen(false)} />
      )}
    </div>
  );
}