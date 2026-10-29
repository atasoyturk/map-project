export interface ReportCategorySelection {
  competitorId:  number;
  complementIds: number[];
}

export const MAX_COMPLEMENTS = 5;

const decimal = new Intl.NumberFormat("tr-TR", { maximumFractionDigits: 1 });

export function formatMeters(meters: number): string {
  return meters < 1000 ? `${Math.round(meters)} m` : `${decimal.format(meters / 1000)} km`;
}

export function formatArea(squareMeters: number): string {
  return `${decimal.format(squareMeters / 1_000_000)} km²`;
}

export function formatWeight(weight: number): string {
  return `%${Math.round(weight * 100)}`;
}

export function formatSaturationRatio(ratio: number | null): string {
  return ratio === null ? "—" : decimal.format(ratio);
}

/** Validates a stored selection against current categories; returns null when unusable. */
export function parseCategorySelection(raw: string | null, validIds: ReadonlySet<number>): ReportCategorySelection | null {
  if (!raw) return null;

  let value: unknown;
  try { value = JSON.parse(raw); } catch { return null; }

  if (typeof value !== "object" || value === null) return null;
  const { competitorId, complementIds } = value as Record<string, unknown>;
  if (typeof competitorId !== "number" || !validIds.has(competitorId) || !Array.isArray(complementIds)) return null;

  const complements = [...new Set(complementIds)]
    .filter((id): id is number => typeof id === "number" && validIds.has(id) && id !== competitorId)
    .slice(0, MAX_COMPLEMENTS);

  return { competitorId, complementIds: complements };
}
