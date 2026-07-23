import { useEffect, useState } from "react";
import OlMap                   from "ol/Map";
import MapBrowserEvent         from "ol/MapBrowserEvent";
import type VectorLayer        from "ol/layer/Vector";
import type VectorSource       from "ol/source/Vector";

export interface RouteClickInfo {
  routeId: number;
  x: number;
  y: number;
}

interface UseTransitRouteClickOptions {
  map:            OlMap | null;
  routeLineLayer: VectorLayer<VectorSource> | null;
  enabled:        boolean;
}

export function useTransitRouteClick({ map, routeLineLayer, enabled }: UseTransitRouteClickOptions) {
  const [selected, setSelected] = useState<RouteClickInfo | null>(null);

  useEffect(() => {
    if (!map || !routeLineLayer) return;
    if (!enabled) { setSelected(null); return; }

    function handleClick(evt: MapBrowserEvent) {
      const feature = map!.forEachFeatureAtPixel(
        evt.pixel,
        (f) => f,
        { layerFilter: (l) => l === routeLineLayer },
      );

      const routeId = feature?.get("routeId");
      if (typeof routeId !== "number") { setSelected(null); return; }

      setSelected({ routeId, x: evt.pixel[0], y: evt.pixel[1] });
    }

    map.on("singleclick", handleClick);
    return () => {
      map.un("singleclick", handleClick);
      setSelected(null);
    };
  }, [map, routeLineLayer, enabled]);

  return { selected, clear: () => setSelected(null) };
}