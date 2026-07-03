import { useState }          from "react";
import Map                   from "ol/Map";
import { MapView }           from "../components/MapView";
import { Navbar }            from "../components/Navbar";
import { InfoPopup }         from "../components/InfoPopup";
import { useSelect }         from "../hooks/useSelect";
import type { SelectedFeatureInfo } from "../hooks/useSelect";
import { buildStyle }        from "../utils/mapStyle";
import type { DrawType }     from "../types/drawing";

export function DashboardPage() {
  const [map,            setMap]           = useState<Map | null>(null);
  const [selected,       setSelected]      = useState<SelectedFeatureInfo | null>(null);
  const [analysisActive, setAnalysisActive] = useState(false);
  const [activeType,     setActiveType]     = useState<DrawType | null>(null);  

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
      />
      <div style={{ marginTop: 56, flex: 1 }}>
        <MapView onMapReady={setMap} height="calc(100vh - 56px)" />
      </div>
      {selected && (
        <InfoPopup
          info={selected}
          onClose={() => setSelected(null)}
          onUpdated={(name, color) => {
            selected.feature.setStyle(buildStyle(color, name));
            setSelected(null);
          }}
        />
      )}
    </div>
  );
}