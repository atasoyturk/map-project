import { useEffect } from "react";
import { Feature }   from "ol";
import Map           from "ol/Map";
import VectorSource  from "ol/source/Vector";
import { GeoJSON }   from "ol/format";
import { Style }     from "ol/style";

interface FeatureLoaderOptions {
  map:          Map | null;
  pointSource:  VectorSource;
  lineSource:   VectorSource;
  polygonSource:VectorSource;
  apiFetch:     (path: string, options?: RequestInit) => Promise<Response>;
  buildStyle:   (color: string, name: string) => Style;
}

const geoJsonFormat = new GeoJSON();

async function loadLayer(
  apiFetch:    (path: string, options?: RequestInit) => Promise<Response>,
  typeName:    string,
  source:      VectorSource,
  type:        "point" | "line" | "polygon",
  buildStyle:  (color: string, name: string) => Style,
): Promise<void> {
  const res = await apiFetch(`/api/proxy/geoserver/wfs?typeName=${typeName}`);
  if (!res.ok) return;

  const geojson = await res.json();

  const features = geoJsonFormat.readFeatures(geojson, {
    dataProjection:    "EPSG:4326",
    featureProjection: "EPSG:3857",
  });

  for (const f of features as Feature[]) {
    const props = f.getProperties();

    // GeoServer columns check
    // GeoServer "tbl_point.8" → 8
    const rawId = props["Id"] ?? props["id"] ?? f.getId();
    const id    = typeof rawId === "string" && rawId.includes(".")
      ? parseInt(rawId.split(".")[1])
      : rawId;
    const name  = props["Name"]  ?? props["name"]  ?? "";
    const color = props["Color"] ?? props["color"] ?? "#3b82f6";

    f.set("id",     id);
    f.set("type",   type);
    f.set("name",   name);
    f.set("color",  color);
    f.set("source", source);
    f.setStyle(buildStyle(color, name));

    source.addFeature(f);
  }
}

export function useFeatureLoader({
  map,
  pointSource,
  lineSource,
  polygonSource,
  apiFetch,
  buildStyle,
}: FeatureLoaderOptions) {
  useEffect(() => {
    if (!map) return;

    async function load() {
      try {
        await Promise.all([
          loadLayer(apiFetch, "tbl_point",   pointSource,   "point",   buildStyle),
          loadLayer(apiFetch, "tbl_line",    lineSource,    "line",    buildStyle),
          loadLayer(apiFetch, "tbl_polygon", polygonSource, "polygon", buildStyle),
        ]);
      } catch { }
    }

    load();
  }, [map]);
}