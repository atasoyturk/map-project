import { useEffect, useRef } from "react";
import Map from "ol/Map";
import Select from "ol/interaction/Select";
import Modify from "ol/interaction/Modify";
import { click } from "ol/events/condition";
import type { Feature } from "ol";

interface SelectedFeatureInfo {
  feature: Feature;
  id:      number;
  type:    "point" | "line" | "polygon";
  name:    string;
  color:   string;
}

interface UseSelectOptions {
  map:        Map | null;
  onSelect:   (info: SelectedFeatureInfo | null) => void;
}

export function useSelect({ map, onSelect }: UseSelectOptions) {
  const selectRef = useRef<Select | null>(null);
  const modifyRef = useRef<Modify | null>(null);

  useEffect(() => {
    if (!map) return;

    const select = new Select({ condition: click });
    const modify = new Modify({ features: select.getFeatures() });

    select.on("select", (e) => {
      const feature = e.selected[0];
      if (!feature) { onSelect(null); return; }

      onSelect({
        feature,
        id:    feature.get("id"),
        type:  feature.get("type"),
        name:  feature.get("name")  ?? "",
        color: feature.get("color") ?? "#3b82f6",
      });
    });

    map.addInteraction(select);
    map.addInteraction(modify);

    selectRef.current = select;
    modifyRef.current = modify;

    return () => {
      map.removeInteraction(select);  // cleanup 
      map.removeInteraction(modify);
      selectRef.current = null;
      modifyRef.current = null;
    };
  }, [map]);
}

export type { SelectedFeatureInfo };