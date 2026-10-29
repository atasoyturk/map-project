import { useEffect } from "react";
import type OlMap  from "ol/Map";
import VectorSource from "ol/source/Vector";
import VectorLayer  from "ol/layer/Vector";
import { Style, Icon } from "ol/style";

export function useSearchMarkerLayer(map: OlMap | null, source: VectorSource) {
  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({
      source,
      updateWhileAnimating: true,
      updateWhileInteracting: true,
      style: new Style({
        image: new Icon({
          anchor: [0.5, 1],
          anchorXUnits: "fraction",
          anchorYUnits: "fraction",
          src: 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="%23030c21" stroke="%23ffffff" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3" fill="white"></circle></svg>',
          scale: 0.7,
        }),
      }),
      zIndex: 200,
    });

    map.addLayer(layer);

    return () => { map.removeLayer(layer); };
  }, [map, source]);
}
