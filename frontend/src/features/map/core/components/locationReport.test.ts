import { describe, expect, it } from "vitest";
import { formatArea, formatMeters, formatSaturationRatio, formatWeight, parseCategorySelection } from "./locationReport";

describe("formatters", () => {
  it("formats meters below 1 km and kilometers above", () => {
    expect(formatMeters(199.6)).toBe("200 m");
    expect(formatMeters(5552)).toBe("5,6 km");
  });

  it("formats area in km² and weights as percent", () => {
    expect(formatArea(12_340_000)).toBe("12,3 km²");
    expect(formatWeight(0.35)).toBe("%35");
  });

  it("formats the saturation ratio or a dash when there are no complements", () => {
    expect(formatSaturationRatio(0.25)).toBe("0,3");
    expect(formatSaturationRatio(null)).toBe("—");
  });
});

describe("parseCategorySelection", () => {
  const valid = new Set([3, 10, 12, 13]);

  it("returns the stored selection when all ids are valid", () => {
    expect(parseCategorySelection('{"competitorId":3,"complementIds":[10,13]}', valid))
      .toEqual({ competitorId: 3, complementIds: [10, 13] });
  });

  it("drops unknown, duplicate and competitor ids from complements", () => {
    expect(parseCategorySelection('{"competitorId":3,"complementIds":[99,12,12,3,"x"]}', valid))
      .toEqual({ competitorId: 3, complementIds: [12] });
  });

  it("returns null for missing, malformed or unknown competitor values", () => {
    expect(parseCategorySelection(null, valid)).toBeNull();
    expect(parseCategorySelection("{oops", valid)).toBeNull();
    expect(parseCategorySelection("42", valid)).toBeNull();
    expect(parseCategorySelection('{"competitorId":99,"complementIds":[10]}', valid)).toBeNull();
  });
});
