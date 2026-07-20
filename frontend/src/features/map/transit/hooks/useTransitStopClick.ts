import { useEffect, useState } from "react";
import OlMap                   from "ol/Map";
import MapBrowserEvent         from "ol/MapBrowserEvent";
import type { Feature }        from "ol";
import type VectorLayer        from "ol/layer/Vector";
import type VectorSource       from "ol/source/Vector";

export interface StopClickInfo {
  feature: Feature;
}

interface UseTransitStopClickOptions {
  map:       OlMap | null;
  stopLayer: VectorLayer<VectorSource> | null;
  enabled:   boolean;
}

export function useTransitStopClick({ map, stopLayer, enabled }: UseTransitStopClickOptions) {
  const [selected, setSelected] = useState<StopClickInfo | null>(null);

  useEffect(() => {
    if (!map || !stopLayer) return;
    if (!enabled) { setSelected(null); return; }

    function handleClick(evt: MapBrowserEvent) {
      const feature = map!.forEachFeatureAtPixel(
        evt.pixel,
        (f) => f as Feature,
        { layerFilter: (l) => l === stopLayer },
      );

      if (!feature || typeof feature.get("stopId") !== "number") {
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
  }, [map, stopLayer, enabled]);

  return { selected, clear: () => setSelected(null) };
}