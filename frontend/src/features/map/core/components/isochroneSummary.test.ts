import { describe, expect, it } from "vitest";
import { buildBandSummaries } from "./isochroneSummary";

describe("buildBandSummaries", () => {
  it("sorts bands ascending, titles them cumulatively and puts stops first", () => {
    const summaries = buildBandSummaries([
      {
        minutesThreshold: 10, polygonWkt: "", stopCount: 2,
        categoryCounts: [{ categoryId: 3, categoryName: "Kafe", count: 2 }, { categoryId: 6, categoryName: "Hastane", count: 1 }],
      },
      { minutesThreshold: 5, polygonWkt: "", stopCount: 0, categoryCounts: [] },
    ]);

    expect(summaries).toEqual([
      { minutes: 5,  title: "5 dk içinde",  rows: [{ key: "stops", label: "Durak", count: 0 }] },
      {
        minutes: 10, title: "10 dk içinde",
        rows: [
          { key: "stops",      label: "Durak",   count: 2 },
          { key: "category-3", label: "Kafe",    count: 2 },
          { key: "category-6", label: "Hastane", count: 1 },
        ],
      },
    ]);
  });
});
