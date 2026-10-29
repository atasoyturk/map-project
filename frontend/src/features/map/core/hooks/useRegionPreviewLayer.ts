import { useEffect, useRef } from "react";
import type OlMap  from "ol/Map";
import Feature      from "ol/Feature";
import VectorSource from "ol/source/Vector";
import VectorLayer  from "ol/layer/Vector";
import WKT          from "ol/format/WKT";
import { Style, Fill, Stroke } from "ol/style";

export function useRegionPreviewLayer(map: OlMap | null) {
  const layerRef = useRef<VectorLayer<VectorSource> | null>(null);

  function clear() {
    if (layerRef.current && map) {
      map.removeLayer(layerRef.current);
      layerRef.current = null;
    }
  }

  function show(wkt: string) {
    clear();

    const geometry = new WKT().readGeometry(wkt, {
      dataProjection:    "EPSG:4326",
      featureProjection: "EPSG:3857",
    });

    const source = new VectorSource({ features: [new Feature({ geometry })] });
    const layer  = new VectorLayer({
      source,
      style: new Style({
        fill:   new Fill({ color: "rgba(59,130,246,0.2)" }),
        stroke: new Stroke({ color: "#3b82f6", width: 2 }),
      }),
      zIndex: 50,
      updateWhileAnimating: true,
      updateWhileInteracting: true,
    });

    map?.addLayer(layer);
    layerRef.current = layer;

    map?.getView().fit(geometry.getExtent(), {
      padding: [60, 60, 60, 60],
      maxZoom: 12,
      duration: 500,
    });
  }

  useEffect(() => {
    if (!map) return;
    return () => {
      if (layerRef.current) {
        map.removeLayer(layerRef.current);
        layerRef.current = null;
      }
    };
  }, [map]);

  return { show, clear };
}
