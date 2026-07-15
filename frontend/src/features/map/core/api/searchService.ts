import type { ApiFetch } from "./geoService";

export interface DrawingSearchParams {
  name?:      string;
  startDate?: string;
  endDate?:   string;
  sortBy:     string;
  sortOrder:  string;
}

/** search for geo shapes with both name and date*/
export function searchDrawings(apiFetch: ApiFetch, params: DrawingSearchParams) {
  const query = new URLSearchParams();
  if (params.name)      query.set("name",      params.name);
  if (params.startDate) query.set("startDate", params.startDate);
  if (params.endDate)   query.set("endDate",   params.endDate);
  query.set("sortBy",    params.sortBy);
  query.set("sortOrder", params.sortOrder);

  return apiFetch(`/api/drawings/search?${query.toString()}`);
}