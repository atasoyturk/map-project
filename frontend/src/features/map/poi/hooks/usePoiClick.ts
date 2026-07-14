import { useEffect, useState } from "react";
import OlMap                   from "ol/Map";
import MapBrowserEvent         from "ol/MapBrowserEvent";
import type { Feature }        from "ol";
import type VectorLayer        from "ol/layer/Vector";
import type VectorSource       from "ol/source/Vector";

export interface PoiClickInfo {
  feature: Feature;
}

interface UsePoiClickOptions {
  map:      OlMap | null;
  poiLayer: VectorLayer<VectorSource> | null;
  enabled:  boolean;
}

export function usePoiClick({ map, poiLayer, enabled }: UsePoiClickOptions) {
  const [selected, setSelected] = useState<PoiClickInfo | null>(null);

  useEffect(() => {
    if (!map || !poiLayer) return;
    if (!enabled) { setSelected(null); return; }

    function handleClick(evt: MapBrowserEvent) {
      const feature = map!.forEachFeatureAtPixel(
        evt.pixel,
        (f) => f as Feature,
        { layerFilter: (l) => l === poiLayer },
      );

      if (!feature || typeof feature.get("poiId") !== "number") {
        setSelected(null);
        return;
      }
      setSelected({ feature });
    }

    map.on("singleclick", handleClick);
    return () => {
      map.un("singleclick", handleClick);
      setSelected(null);
    };
  }, [map, poiLayer, enabled]);

  return { selected, clear: () => setSelected(null) };
}