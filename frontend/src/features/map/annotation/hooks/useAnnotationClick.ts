import { useEffect, useState } from "react";
import Map from "ol/Map";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { Select } from "ol/interaction";
import { click } from "ol/events/condition";
import Feature from "ol/Feature";

interface UseAnnotationClickProps {
  map: Map | null;
  annotationLayer: VectorLayer<VectorSource> | null;
  enabled: boolean;
}

export function useAnnotationClick({ map, annotationLayer, enabled }: UseAnnotationClickProps) {
  const [selected, setSelected] = useState<{ feature: Feature; x: number; y: number } | null>(null);

  useEffect(() => {
    if (!map || !annotationLayer || !enabled) return;

    const selectInteraction = new Select({
      condition: click,
      layers: [annotationLayer],
      style: null, // Mevcut stili bozma
    });

    selectInteraction.on("select", (e) => {
      const feature = e.selected[0];
      if (feature) {
        const pixel = map.getPixelFromCoordinate(feature.getGeometry()?.getClosestPoint(map.getView().getCenter()!)!);
        setSelected({
          feature,
          x: pixel[0],
          y: pixel[1],
        });
      } else {
        setSelected(null);
      }
    });

    map.addInteraction(selectInteraction);

    return () => {
      map.removeInteraction(selectInteraction);
    };
  }, [map, annotationLayer, enabled]);

  return {
    selected,
    clear: () => setSelected(null),
  };
}
