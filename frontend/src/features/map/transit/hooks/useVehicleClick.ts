import { useEffect, useState } from "react";
import OlMap                   from "ol/Map";
import MapBrowserEvent         from "ol/MapBrowserEvent";
import type { Feature }        from "ol";
import type VectorLayer        from "ol/layer/Vector";
import type VectorSource       from "ol/source/Vector";

export interface VehicleClickInfo {
  feature: Feature;
  x: number;
  y: number;
}

interface UseVehicleClickOptions {
  map:          OlMap | null;
  vehicleLayer: VectorLayer<VectorSource> | null;
  enabled:      boolean;
}

export function useVehicleClick({ map, vehicleLayer, enabled }: UseVehicleClickOptions) {
  const [selected, setSelected] = useState<VehicleClickInfo | null>(null);

  useEffect(() => {
    if (!map || !vehicleLayer) return;
    if (!enabled) { setSelected(null); return; }

    function handleClick(evt: MapBrowserEvent) {
      const feature = map!.forEachFeatureAtPixel(
        evt.pixel,
        (f) => f as Feature,
        { layerFilter: (l) => l === vehicleLayer },
      );

      if (!feature) { setSelected(null); return; }
      setSelected({ feature, x: evt.pixel[0], y: evt.pixel[1] });
    }

    map.on("singleclick", handleClick);
    return () => {
      map.un("singleclick", handleClick);
      setSelected(null);
    };
  }, [map, vehicleLayer, enabled]);

  return { selected, clear: () => setSelected(null) };
}