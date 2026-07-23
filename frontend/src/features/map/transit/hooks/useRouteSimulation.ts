import { useEffect, useRef, useState, useCallback } from "react";
import type { HubConnection } from "@microsoft/signalr";
import {
  startRouteSimulation, stopRouteSimulation, getRouteSimulationStatus,
} from "../../../../shared/api/transitService";

export interface VehiclePosition {
  routeId:             number;
  latitude:            number;
  longitude:           number;
  progressPercentage:  number;
  completed:           boolean;
}

type ApiFetch = (path: string, options?: RequestInit) => Promise<Response>;

export function useRouteSimulation(connection: HubConnection | null, apiFetch: ApiFetch) {
  const [trackedRouteId,  setTrackedRouteId]  = useState<number | null>(null);
  const [vehiclePosition, setVehiclePosition] = useState<VehiclePosition | null>(null);
  const trackedRouteIdRef = useRef<number | null>(null);

  useEffect(() => { trackedRouteIdRef.current = trackedRouteId; }, [trackedRouteId]);

  useEffect(() => {
    if (!connection) return;

    function handlePosition(dto: VehiclePosition) {
      if (dto.routeId !== trackedRouteIdRef.current) return;
      setVehiclePosition(dto);
    }

    function handleStopped(payload: { routeId: number }) {
      if (payload.routeId !== trackedRouteIdRef.current) return;
      setVehiclePosition(null);
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

    try {
      const res = await getRouteSimulationStatus(apiFetch, routeId);
      if (res.ok) {
        const data = await res.json().catch(() => null);
        if (data) setVehiclePosition(data);
      }
    } catch { /* durum bilgisi alınamadı, canlı güncellemeler yine de gelmeye devam eder */ }
  }, [connection, apiFetch]);

  const stopTracking = useCallback(async () => {
    if (connection && trackedRouteId !== null && connection.state === "Connected") {
      await connection.invoke("LeaveRoute", trackedRouteId).catch(() => {});
    }
    setTrackedRouteId(null);
    setVehiclePosition(null);
  }, [connection, trackedRouteId]);

  const startSimulation = useCallback(
    (routeId: number) => startRouteSimulation(apiFetch, routeId),
    [apiFetch],
  );

  const stopSimulation = useCallback(
    (routeId: number) => stopRouteSimulation(apiFetch, routeId),
    [apiFetch],
  );

  return { trackedRouteId, vehiclePosition, startTracking, stopTracking, startSimulation, stopSimulation };
}