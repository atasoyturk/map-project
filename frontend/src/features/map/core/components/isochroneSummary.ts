import type { IsochroneAnalysisBandDto } from "../api/thematicMapService";

export interface SummaryRow {
  key:   string;
  label: string;
  count: number;
}

export interface BandSummary {
  minutes: number;
  title:   string;
  rows:    SummaryRow[];
}

/** Smallest band first; stops on top, then categories in the backend's order (count desc). */
export function buildBandSummaries(bands: IsochroneAnalysisBandDto[]): BandSummary[] {
  return [...bands]
    .sort((a, b) => a.minutesThreshold - b.minutesThreshold)
    .map((band) => ({
      minutes: band.minutesThreshold,
      title:   `${band.minutesThreshold} dk içinde`,
      rows: [
        { key: "stops", label: "Durak", count: band.stopCount },
        ...band.categoryCounts.map((c) => ({ key: `category-${c.categoryId}`, label: c.categoryName, count: c.count })),
      ],
    }));
}
