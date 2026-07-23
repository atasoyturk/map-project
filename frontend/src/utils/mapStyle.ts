import { Style, Fill, Stroke, Circle, Text, RegularShape, Icon} from "ol/style";
import type { FeatureLike } from "ol/Feature";
import LineString from "ol/geom/LineString";
import Point from "ol/geom/Point";

export function hexToRgba(hex: string, alpha: number): string {
  const r = parseInt(hex.slice(1, 3), 16);
  const g = parseInt(hex.slice(3, 5), 16);
  const b = parseInt(hex.slice(5, 7), 16);
  return `rgba(${r},${g},${b},${alpha})`;
}

export function buildStyle(color: string, name: string): Style {
  const c = color || "#3b82f6";
  return new Style({
    fill:   new Fill({ color: hexToRgba(c, 0.2) }),
    stroke: new Stroke({ color: c, width: 2 }),
    image:  new Circle({
      radius: 6,
      fill:   new Fill({ color: c }),
      stroke: new Stroke({ color: "#ffffff", width: 2 }),
    }),
    text: name ? new Text({
      text:     name,
      font:     "bold 13px sans-serif",
      fill:     new Fill({ color: "#0f172a" }),
      stroke:   new Stroke({ color: "#ffffff", width: 3 }),
      offsetY:  -16,
      overflow: true,
    }) : undefined,
  });
}

const ROUTE_ARROW_COUNT = 4;

function buildArrowStyles(geometry: LineString, color: string): Style[] {
  const length = geometry.getLength();
  if (length === 0) return [];

  const arrows: Style[] = [];
  const sampleDelta = 0.01;

  for (let i = 1; i <= ROUTE_ARROW_COUNT; i++) {
    const fraction     = i / (ROUTE_ARROW_COUNT + 1);
    const point        = geometry.getCoordinateAt(fraction);
    const aheadFraction = Math.min(fraction + sampleDelta, 1);
    const aheadPoint    = geometry.getCoordinateAt(aheadFraction);

    const angle = Math.atan2(aheadPoint[1] - point[1], aheadPoint[0] - point[0]);

    arrows.push(new Style({
      geometry: new Point(point),
      image: new RegularShape({
        points:  3,
        radius:  7,
        fill:    new Fill({ color }),
        stroke:  new Stroke({ color: "#ffffff", width: 1 }),
    
        rotation: -angle + Math.PI / 2,
      }),
    }));
  }

  return arrows;
}

export function buildRouteStyle(color: string) {
  return function routeStyleFn(feature: FeatureLike): Style[] {
    const geometry = feature.getGeometry();
    if (!(geometry instanceof LineString)) return [];

    return [
      new Style({ stroke: new Stroke({ color, width: 4 }) }),
      ...buildArrowStyles(geometry, color),
    ];
  };
}

export function buildVehicleStyle(): Style {
  return new Style({
    image: new Icon({
      src: 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="%23f59e0b" stroke="%230f172a" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M10 17h4V5H2v12h3"/><path d="M20 17h2v-3.34a4 4 0 0 0-1.17-2.83L19 9h-5"/><circle cx="7.5" cy="17.5" r="2.5"/><circle cx="17.5" cy="17.5" r="2.5"/></svg>',
      scale: 0.5,
      anchor: [0.5, 0.5],
    }),
  });
}