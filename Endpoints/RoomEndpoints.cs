using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;
using SmartHomeRepo.DTO;

namespace SmartHomeRepo.Endpoints;


public static partial class RoomEndpoints
{

public static async Task<IResult> GetAllRooms(AppDbContext db, ILogger<LogCategory> logger)
    {
        if (db.Rooms == null)
        {
            logger.LogError("Ошибка доступа к базе данных: таблица Rooms не найдена.");
            return Results.Problem("Внутренняя ошибка сервера: таблица Rooms не найдена.");
        }

        var rooms = await db.Rooms.Select(r => new FlatRoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            RoomType = r.RoomType,
            ApartmentId = r.ApartmentId
        }).ToListAsync();

        logger.LogInformation("Получено {Count} комнат из базы данных.", rooms.Count);
        logger.LogInformation("Отправляю список комнат клиенту.");

        return Results.Ok(rooms); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

    public static async Task<IResult> GetRoomInfo(int id, AppDbContext db, ILogger<LogCategory> logger)
    {
        if (db.Rooms == null)
        {
            logger.LogError("Ошибка доступа к базе данных: таблица Apartments не найдена.");
            return Results.Problem("Внутренняя ошибка сервера: таблица Apartments не найдена.");
        }

        var rooms = await db.Rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            RoomType = r.RoomType,
            Area = r.Info != null ? r.Info.Area : 0,
            Temperature = r.Info != null ? r.Info.Temperature : 0,
        }).ToListAsync();

        logger.LogInformation("Получено {Count} комнат из базы данных.", rooms.Count);
        logger.LogInformation("Отправляю список комнат клиенту.");

        return Results.Ok(rooms); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

}



public static partial class RoomEndpoints
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/rooms").WithTags("Rooms");

        group.MapGet("/", GetAllRooms)
            .WithSummary("Список комнат")
            .WithDescription("Получить список всех комнат во всех квартирах. Название, описание, тип комнаты и Id квартиры.")
            .Produces<List<FlatRoomDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}/info", GetRoomInfo)
            .WithSummary("Датчики комнаты")
            .WithDescription("Получить информацию о комнате по её ID (НЕ номеру квартиры). Включает в себя площадь, температуру и состояние света.")
            .Produces<List<RoomDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id}/light", async (int id, AppDbContext db) =>
        {
            var info = await db.RoomInfos.FirstOrDefaultAsync(i => i.RoomId == id);
            if (info == null) return Results.NotFound();

            await Task.Delay(500); // Сетевая задержка
            info.LightState = !info.LightState;
            await db.SaveChangesAsync();
            return Results.Ok(info);
        }).WithSummary("Переключить свет");
    }




}