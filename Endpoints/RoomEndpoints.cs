using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;

namespace SmartHomeRepo.Endpoints;

public static class RoomEndpoints {
    public static void MapRoomEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("/api/rooms").WithTags("Rooms");

        group.MapGet("/{id}/info", async (int id, AppDbContext db) =>
            await db.RoomInfos.FirstOrDefaultAsync(i => i.RoomId == id) 
                is { } info ? Results.Ok(info) : Results.NotFound())
            .WithSummary("Датчики комнаты");

        group.MapPatch("/{id}/light", async (int id, AppDbContext db) => {
            var info = await db.RoomInfos.FirstOrDefaultAsync(i => i.RoomId == id);
            if (info == null) return Results.NotFound();
            
            await Task.Delay(500); // Сетевая задержка
            info.LightState = !info.LightState;
            await db.SaveChangesAsync();
            return Results.Ok(info);
        }).WithSummary("Переключить свет");
    }
}