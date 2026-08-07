import { useEffect, useRef } from "react";
import type OlMap    from "ol/Map";
import Feature        from "ol/Feature";
import VectorSource    from "ol/source/Vector";
import VectorLayer     from "ol/layer/Vector";
import { WKT }          from "ol/format";
import { Style, Fill, Stroke } from "ol/style";
import type { IsochroneResultDto } from "../../../../shared/api/thematicMapService";

const wktFormat = new WKT();

function buildBandStyle(minutes: number, allMinutesSorted: number[]): Style {
  const index    = allMinutesSorted.indexOf(minutes);
  const opacity  = 0.5 - (index / Math.max(allMinutesSorted.length - 1, 1)) * 0.4;

  return new Style({
    fill:   new Fill({ color: `rgba(3, 12, 33, ${opacity})` }),
    stroke: new Stroke({ color: "#030c21", width: 1.5 }),
  });
}

export function useIsochroneLayer(map: OlMap | null, result: IsochroneResultDto | null) {
  const sourceRef = useRef(new VectorSource());

  useEffect(() => {
    if (!map) return;
    const layer = new VectorLayer({ source: sourceRef.current, zIndex: 50, updateWhileAnimating: true, updateWhileInteracting: true });
    map.addLayer(layer);
    return () => { map.removeLayer(layer); };
  }, [map]);

  useEffect(() => {
    const source = sourceRef.current;
    source.clear();

    if (!result) return;

    const allMinutesSorted = result.bands.map((b) => b.minutesThreshold).sort((a, b) => a - b);

    for (const band of result.bands) {
      const geometry = wktFormat.readGeometry(band.polygonWkt, {
        dataProjection:    "EPSG:4326",
        featureProjection: "EPSG:3857",
      });
      const feature = new Feature({ geometry });
      feature.setStyle(buildBandStyle(band.minutesThreshold, allMinutesSorted));
      source.addFeature(feature);
    }
  }, [result]);

  return sourceRef;
}