import { useEffect, useRef } from "react";
import Map from "ol/Map";
import Modify from "ol/interaction/Modify";
import { Collection } from "ol";
import type { Feature } from "ol";
import type { SelectedFeatureInfo } from "./useSelect";

interface UseMapClickOptions {
  map:              Map | null;
  enabled:          boolean;
  onFeaturesFound:  (features: SelectedFeatureInfo[]) => void;
}

function toInfo(feature: Feature): SelectedFeatureInfo | null {
  const id   = feature.get("id");
  const type = feature.get("type");
  if (!id || !type) return null;
  return {
    feature,
    id,
    type,
    name:  feature.get("name")  ?? "",
    color: feature.get("color") ?? "#3b82f6",
  };
}

export function useMapClick({ map, enabled, onFeaturesFound }: UseMapClickOptions) {
  const modifyRef     = useRef<Modify | null>(null);
  const selectedRef   = useRef(new Collection<Feature>());

  useEffect(() => {
    if (!map) return;

    if (!enabled) {
      if (modifyRef.current) { map.removeInteraction(modifyRef.current); modifyRef.current = null; }
      selectedRef.current.clear();
      onFeaturesFound([]);
      return;
    }

    const modify = new Modify({ features: selectedRef.current });
    map.addInteraction(modify);
    modifyRef.current = modify;

    function handleClick(e: any) {
      const found: SelectedFeatureInfo[] = [];
      map!.forEachFeatureAtPixel(e.pixel, (f) => {
        const info = toInfo(f as Feature);
        if (info) found.push(info);
      });
      onFeaturesFound(found);

      // selected features are managed by Modify interaction
      selectedRef.current.clear();
      if (found.length > 0) selectedRef.current.push(found[0].feature);
    }

    map.on("singleclick", handleClick);

    return () => {
      map.un("singleclick", handleClick);
      map.removeInteraction(modify);
      modifyRef.current = null;
      selectedRef.current.clear();
    };
  }, [map, enabled]);
}