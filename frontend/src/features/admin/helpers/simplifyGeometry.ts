import type Geometry from "ol/geom/Geometry";
import MultiPolygon  from "ol/geom/MultiPolygon";
import Polygon       from "ol/geom/Polygon";
import { WKT }       from "ol/format";

/** Backend GeometryConverter.MaxCoordinateCount */
export const MAX_GEOMETRY_POINTS = 10_000;

function countPoints(geometry: Geometry): number {
  return new WKT().writeGeometry(geometry).split(",").length;
}


function dropDegenerateParts(geometry: Geometry): Geometry {
  if (geometry instanceof MultiPolygon) {
    const polys = geometry.getCoordinates()
      .map((poly) => poly.filter((ring) => ring.length >= 4))
      .filter((poly) => poly.length > 0);
    return new MultiPolygon(polys);
  }
  if (geometry instanceof Polygon) {
    return new Polygon(
      geometry.getCoordinates().filter((ring) => ring.length >= 4)
    );
  }
  return geometry;
}

export function simplifyToLimit(geometry: Geometry): Geometry {
  let tolerance = 100;
  let simplified = geometry;
  while (countPoints(simplified) > MAX_GEOMETRY_POINTS && tolerance < 50_000) {
    simplified = dropDegenerateParts(geometry.simplify(tolerance * tolerance));
    tolerance *= 2;
  }
  return simplified;
}