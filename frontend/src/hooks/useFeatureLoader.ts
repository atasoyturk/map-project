import { useEffect } from "react";
import { Feature } from "ol";
import Map from "ol/Map";
import VectorSource from "ol/source/Vector";
import { WKT } from "ol/format";
import { Style } from "ol/style";

interface FeatureLoaderOptions {
  map:        Map | null;
  source:     VectorSource;
  apiFetch:   (path: string, options?: RequestInit) => Promise<Response>;
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

        for (const p of points) {
          const geom = wktFormat.readGeometry(p.wktGeometry, {
            dataProjection:    "EPSG:4326",
            featureProjection: "EPSG:3857",
          });
          const f = new Feature({ geometry: geom });
          f.set("id",   p.id);       
          f.set("type", "point");    
          f.set("name", p.name);     
          f.set("color", p.color);   
          f.setStyle(buildStyle(p.color, p.name));
          features.push(f);
        }

        for (const l of lines) {
          const geom = wktFormat.readGeometry(l.wktGeometry, {
            dataProjection:    "EPSG:4326",
            featureProjection: "EPSG:3857",
          });
          const f = new Feature({ geometry: geom });
          f.set("id",   l.id);
          f.set("type", "line");
          f.set("name", l.name);
          f.set("color", l.color);
          f.setStyle(buildStyle(l.color, l.name));
          features.push(f);
        }

        for (const p of polygons) {
          const geom = wktFormat.readGeometry(p.wktGeometry, {
            dataProjection:    "EPSG:4326",
            featureProjection: "EPSG:3857",
          });
          const f = new Feature({ geometry: geom });
          f.set("id",   p.id);
          f.set("type", "polygon");
          f.set("name", p.name);
          f.set("color", p.color);
          f.setStyle(buildStyle(p.color, p.name));
          features.push(f);
        }

        source.addFeatures(features);
      } catch {
        // sessiz hata — harita yine de çalışır
      }
    }

    load();
  }, [map]);
}