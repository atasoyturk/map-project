import { useEffect, useRef, useState, useCallback } from "react";
import type { HubConnection } from "@microsoft/signalr";
import {
  startRouteSimulation, stopRouteSimulation, getRouteSimulationStatus,
} from "../../../../shared/api/transitService";

export interface VehiclePosition {
  routeId:             number;
  vehicleId:           number;
  plateNumber:         string;
  latitude:            number;
  longitude:           number;
  progressPercentage:  number;
  completed:           boolean;
}

export interface VehicleActionResult {
  vehicleId:   number;
  plateNumber: string;
  success:     boolean;
  error:       string | null;
}

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

export function useRouteSimulation(connection: HubConnection | null, apiFetch: ApiFetch) {
  const [trackedRouteId, setTrackedRouteId] = useState<number | null>(null);
  const [vehiclePositions, setVehiclePositions] = useState<Map<number, VehiclePosition>>(new Map());
  const trackedRouteIdRef = useRef<number | null>(null);

  useEffect(() => { trackedRouteIdRef.current = trackedRouteId; }, [trackedRouteId]);

  useEffect(() => {
    if (!connection) return;

    function handlePosition(dto: VehiclePosition) {
      if (dto.routeId !== trackedRouteIdRef.current) return;

      setVehiclePositions((prev) => {
        const next = new Map(prev);
        if (dto.completed) next.delete(dto.vehicleId);
        else next.set(dto.vehicleId, dto);
        return next;
      });
    }

    function handleStopped(payload: { routeId: number; vehicleId: number }) {
      if (payload.routeId !== trackedRouteIdRef.current) return;

      setVehiclePositions((prev) => {
        const next = new Map(prev);
        next.delete(payload.vehicleId);
        return next;
      });
    }

    connection.on("VehiclePositionUpdated", handlePosition);
    connection.on("SimulationStopped", handleStopped);

    return () => {
      connection.off("VehiclePositionUpdated", handlePosition);
      connection.off("SimulationStopped", handleStopped);
    };
  }, [connection]);

  const startTracking = useCallback(async (routeId: number) => {
    if (!connection) return;

    if (connection.state === "Disconnected") {
      try { await connection.start(); } catch { return; }
    }

    await connection.invoke("JoinRoute", routeId);
    setTrackedRouteId(routeId);
    setVehiclePositions(new Map());

    try {
      const res = await getRouteSimulationStatus(apiFetch, routeId);
      if (res.ok) {
        const data: VehiclePosition[] = await res.json().catch(() => []);
        setVehiclePositions(new Map(data.map((p) => [p.vehicleId, p])));
      }
    } catch { /* durum bilgisi alınamadı, canlı güncellemeler yine de gelmeye devam eder */ }
  }, [connection, apiFetch]);

  const stopTracking = useCallback(async () => {
    if (connection && trackedRouteId !== null && connection.state === "Connected") {
      await connection.invoke("LeaveRoute", trackedRouteId).catch(() => {});
    }
    setTrackedRouteId(null);
    setVehiclePositions(new Map());
  }, [connection, trackedRouteId]);

  const startSimulation = useCallback(
    async (routeId: number, vehicleIds: number[]): Promise<VehicleActionResult[]> => {
      const res = await startRouteSimulation(apiFetch, routeId, vehicleIds);
      if (!res.ok) return vehicleIds.map((id) => ({ vehicleId: id, plateNumber: "", success: false, error: "İstek başarısız oldu." }));
      const data = await res.json();
      return data.results as VehicleActionResult[];
    },
    [apiFetch],
  );

  const stopSimulation = useCallback(
    async (routeId: number, vehicleIds: number[]): Promise<VehicleActionResult[]> => {
      const res = await stopRouteSimulation(apiFetch, routeId, vehicleIds);
      if (!res.ok) return vehicleIds.map((id) => ({ vehicleId: id, plateNumber: "", success: false, error: "İstek başarısız oldu." }));
      const data = await res.json();
      return data.results as VehicleActionResult[];
    },
    [apiFetch],
  );

  const fetchRunningVehicleIds = useCallback(async (routeId: number): Promise<Set<number>> => {
    try {
      const res = await getRouteSimulationStatus(apiFetch, routeId);
      if (!res.ok) return new Set();
      const data: VehiclePosition[] = await res.json();
      return new Set(data.map((p) => p.vehicleId));
    } catch {
      return new Set();
    }
  }, [apiFetch]);

  return {
    trackedRouteId, vehiclePositions,
    startTracking, stopTracking,
    startSimulation, stopSimulation,
    fetchRunningVehicleIds,
  };
}