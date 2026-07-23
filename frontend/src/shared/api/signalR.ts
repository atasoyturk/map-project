import * as signalR from "@microsoft/signalr";

const HUB_URL = "http://localhost:5130/hubs/route-simulation";

export function createRouteSimulationConnection(): signalR.HubConnection {
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => localStorage.getItem("token") ?? "",
    })
    .withAutomaticReconnect()
    .build();
}