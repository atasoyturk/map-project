import { describe, expect, it } from "vitest";
import { transformExtent } from "ol/proj";
import { extentToBbox, formatOsmImportResult, isBboxTooLarge } from "./osmImport";

describe("extentToBbox", () => {
  it("converts an EPSG:3857 extent to lon/lat", () => {
    const extent = transformExtent([29.0, 40.9, 29.1, 41.0], "EPSG:4326", "EPSG:3857");

    const bbox = extentToBbox(extent);

    expect(bbox.minLon).toBeCloseTo(29.0);
    expect(bbox.minLat).toBeCloseTo(40.9);
    expect(bbox.maxLon).toBeCloseTo(29.1);
    expect(bbox.maxLat).toBeCloseTo(41.0);
  });
});

describe("isBboxTooLarge", () => {
  it("accepts exactly 0.5° × 0.5°", () => {
    expect(isBboxTooLarge({ minLon: 29, minLat: 40.5, maxLon: 29.5, maxLat: 41 })).toBe(false);
  });

  it("rejects a bbox wider than 0.5°", () => {
    expect(isBboxTooLarge({ minLon: 29, minLat: 40.9, maxLon: 29.6, maxLat: 41 })).toBe(true);
  });

  it("rejects a bbox taller than 0.5°", () => {
    expect(isBboxTooLarge({ minLon: 29, minLat: 40.4, maxLon: 29.1, maxLat: 41 })).toBe(true);
  });
});

describe("formatOsmImportResult", () => {
  it("lists the counts", () => {
    expect(formatOsmImportResult({ added: 12, updated: 3, skipped: 1, skippedCategories: [] }))
      .toBe("OSM içe aktarma tamamlandı: 12 eklendi, 3 güncellendi, 1 atlandı.");
  });

  it("mentions missing categories", () => {
    expect(formatOsmImportResult({ added: 12, updated: 3, skipped: 1, skippedCategories: [4, 7] }))
      .toBe("OSM içe aktarma tamamlandı: 12 eklendi, 3 güncellendi, 1 atlandı (2 kategori artık mevcut olmadığı için atlandı).");
  });
});
