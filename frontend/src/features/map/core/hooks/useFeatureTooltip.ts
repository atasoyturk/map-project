import { useEffect, useState } from "react";
import OlMap                   from "ol/Map";
import MapBrowserEvent         from "ol/MapBrowserEvent";
import type { Feature }        from "ol";
import type VectorLayer        from "ol/layer/Vector";
import type VectorSource       from "ol/source/Vector";
import type { UserLookupEntry } from "./useUserLookup";

export interface TooltipInfo {
  email:       string;
  teamName:    string | null;
  createdDate: string | null;
  x: number;
  y: number;
}

interface HoverableLayers {
  pointLayer:   VectorLayer<VectorSource>;
  lineLayer:    VectorLayer<VectorSource>;
  polygonLayer: VectorLayer<VectorSource>;
}

interface UseFeatureTooltipOptions {
  map:        OlMap | null;
  layers:     HoverableLayers | null;
  userLookup: Map<number, UserLookupEntry>;
  enabled:    boolean;
}

export function useFeatureTooltip({ map, layers, userLookup, enabled }: UseFeatureTooltipOptions) {
  const [tooltip, setTooltip] = useState<TooltipInfo | null>(null);

  useEffect(() => {
    if (!map || !layers) return;
    if (!enabled) { setTooltip(null); return; }

    function handlePointerMove(evt: MapBrowserEvent) {
      if (evt.dragging) { setTooltip(null); return; }

      const feature = map!.forEachFeatureAtPixel(
        evt.pixel,
        (f) => f as Feature,
        {
          layerFilter: (l) =>
            l === layers!.pointLayer || l === layers!.lineLayer || l === layers!.polygonLayer,
        },
      );

      if (!feature) { setTooltip(null); return; }

      const clustered = feature.get("features") as Feature[] | undefined;
      const target = clustered ? (clustered.length === 1 ? clustered[0] : null) : feature;
      if (!target) { setTooltip(null); return; }

      const userId = target.get("userId");
      const entry  = typeof userId === "number" ? userLookup.get(userId) : undefined;
      const createdDate = target.get("createdDate") as string | undefined;

      setTooltip({
        email:       entry?.email ?? "Bilinmiyor",
        teamName:    entry?.teamName ?? null,
        createdDate: createdDate ? new Date(createdDate).toLocaleString("tr-TR") : null,
        x: evt.pixel[0],
        y: evt.pixel[1],
      });
    }

    map.on("pointermove", handlePointerMove);
    return () => {
      map.un("pointermove", handlePointerMove);
      setTooltip(null);
    };
  }, [map, layers, userLookup, enabled]);

  return tooltip;
}