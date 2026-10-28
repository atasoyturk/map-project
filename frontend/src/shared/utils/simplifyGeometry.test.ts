import { describe, expect, it } from "vitest";
import MultiPolygon from "ol/geom/MultiPolygon";
import Polygon      from "ol/geom/Polygon";
import { MAX_GEOMETRY_POINTS, simplifyToLimit } from "./simplifyGeometry";

/** Closed ring of `count` points on a circle, in EPSG:3857 metres. */
function circleRing(count: number, radius: number, cx = 0, cy = 0): number[][] {
  const ring: number[][] = [];
  for (let i = 0; i < count; i++) {
    const a = (2 * Math.PI * i) / count;
    ring.push([cx + radius * Math.cos(a), cy + radius * Math.sin(a)]);
  }
  ring.push(ring[0]);
  return ring;
}

function pointCount(coords: number[][][]): number {
  return coords.reduce((sum, ring) => sum + ring.length, 0);
}

describe("simplifyToLimit", () => {
  it("returns a geometry under the limit unchanged", () => {
    const polygon = new Polygon([circleRing(100, 10_000)]);
    expect(simplifyToLimit(polygon)).toBe(polygon);
  });

  it("reduces a polygon over the limit to at most MAX_GEOMETRY_POINTS", () => {
    const polygon = new Polygon([circleRing(MAX_GEOMETRY_POINTS + 2_000, 10_000)]);

    const result = simplifyToLimit(polygon);

    expect(result).toBeInstanceOf(Polygon);
    const coords = (result as Polygon).getCoordinates();
    expect(pointCount(coords)).toBeLessThanOrEqual(MAX_GEOMETRY_POINTS);
    expect(coords[0].length).toBeGreaterThanOrEqual(4);
  });

  it("drops multipolygon parts that collapse below a valid ring", () => {
    const multi = new MultiPolygon([
      [circleRing(MAX_GEOMETRY_POINTS + 2_000, 10_000)],
      [circleRing(8, 5, 50_000, 50_000)],
    ]);

    const result = simplifyToLimit(multi);

    expect(result).toBeInstanceOf(MultiPolygon);
    const polys = (result as MultiPolygon).getCoordinates();
    expect(polys).toHaveLength(1);
    expect(pointCount(polys[0])).toBeLessThanOrEqual(MAX_GEOMETRY_POINTS);
  });
});
