import { Style, Fill, Stroke, Circle, Text } from "ol/style";

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