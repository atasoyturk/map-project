using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BackendApi.Hubs;

[Authorize]
public sealed class RouteSimulationHub : Hub
{
    public static string GroupName(int routeId) => $"route-{routeId}";

    public Task JoinRoute(int routeId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, GroupName(routeId));

    public Task LeaveRoute(int routeId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(routeId));
}