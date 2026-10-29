import { useEffect } from "react";
import type OlMap   from "ol/Map";
import Feature       from "ol/Feature";
import Point         from "ol/geom/Point";
import { fromLonLat } from "ol/proj";
import VectorSource  from "ol/source/Vector";
import HeatmapLayer  from "ol/layer/Heatmap";

export function useAnalysisHeatmapLayer(
  map: OlMap | null,
  points: { latitude: number; longitude: number; weight: number }[] | null,
) {
  useEffect(() => {
    if (!map || !points) return;

    const vectorSource = new VectorSource();

    points.forEach(pt => {
      const feature = new Feature({
        geometry: new Point(fromLonLat([pt.longitude, pt.latitude])),
        weight: pt.weight
      });
      vectorSource.addFeature(feature);
    });

    const heatmapLayer = new HeatmapLayer({
      source: vectorSource,
      blur: 20,
      radius: 10,
      weight: (feature: Feature) => feature.get("weight"),
      zIndex: 100
    });

    map.addLayer(heatmapLayer);

    return () => {
      map.removeLayer(heatmapLayer);
    };
  }, [map, points]);
}
