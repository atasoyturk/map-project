import { useEffect } from "react";
import type OlMap        from "ol/Map";
import type MapBrowserEvent from "ol/MapBrowserEvent";
import { toLonLat }       from "ol/proj";

interface UseTripPointClickOptions {
  map:      OlMap | null;
  enabled:  boolean;
  onPick:   (lon: number, lat: number) => void;
}

export function useTripPointClick({ map, enabled, onPick }: UseTripPointClickOptions) {
  useEffect(() => {
    if (!map || !enabled) return;

    function handleClick(evt: MapBrowserEvent) {
      const [lon, lat] = toLonLat(evt.coordinate);
      onPick(lon, lat);
    }

    map.on("singleclick", handleClick);
    return () => { map.un("singleclick", handleClick); };
  }, [map, enabled, onPick]);
}