import { useEffect } from "react";
import { Feature } from "ol";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import { Style } from "ol/style";

interface FeatureLoaderOptions {
  map:       Map | null;
  source:    VectorSource;
  apiFetch:  (path: string, options?: RequestInit) => Promise<Response>;
  buildStyle: (color: string, name: string) => Style;
}

export function useFeatureLoader({
  map,
  source,
  apiFetch,
  buildStyle,
}: FeatureLoaderOptions) {
  useEffect(() => {
    if (!map) return;

    const wktFormat = new WKT();

    async function load() {
      try {
        const [pr, lr, polygr] = await Promise.all([
          apiFetch("/api/point"),
          apiFetch("/api/line"),
          apiFetch("/api/polygon"),
        ]);

        if (!pr.ok || !lr.ok || !polygr.ok) return;

        const [points, lines, polygons] = await Promise.all([
          pr.json(),
          lr.json(),
          polygr.json(),
        ]);

        const features: Feature[] = [];

        for (const item of [...points, ...lines, ...polygons]) {
          const geom = wktFormat.readGeometry(item.wktGeometry, {
            dataProjection:    "EPSG:4326",
            featureProjection: "EPSG:3857",
          });
          const f = new Feature({ geometry: geom });
          f.setStyle(buildStyle(item.color, item.name));
          features.push(f);
        }

        source.addFeatures(features);
      } catch {
        
      }
    }

    load();
  }, [map]);
}