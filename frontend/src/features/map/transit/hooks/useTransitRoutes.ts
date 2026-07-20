import { useEffect, useState } from "react";
import { getAllTransitRoutes } from "../../../../shared/api/transitService";
import type { TransitRouteResponseDto } from "../types";

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

export function useTransitRoutes(apiFetch: ApiFetch) {
  const [routes, setRoutes] = useState<TransitRouteResponseDto[]>([]);

  async function reload() {
    try {
      const res = await getAllTransitRoutes(apiFetch);
      if (!res.ok) return;
      setRoutes(await res.json());
    } catch { }
  }

  useEffect(() => { reload(); }, [apiFetch]);

  return { routes, reload };
}