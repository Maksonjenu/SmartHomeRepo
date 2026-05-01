using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;
using SmartHome.Core.DTO;
using SmartHomeRepo.Models.Interfaces;

namespace SmartHomeRepo.Endpoints;


public static partial class RoomEndpoints
{

    #region Handlers

    public static async Task<IResult> GetAllRooms(AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке получить список комнат.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

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

        rooms = network.MessWithData(rooms); // Имитируем порчу данных (может очистить список или занулить поля)

        logger.LogInformation("Получено {Count} комнат из базы данных.", rooms.Count);
        logger.LogInformation("Отправляю список комнат клиенту.");

        return Results.Ok(rooms); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

    public static async Task<IResult> GetRoomInfo(int id, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке получить информацию о комнате. ID: {Id}", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        if (id < 0)
        {
            logger.LogError("Некорректный ID комнаты: {Id}. ID должен быть положительным числом.", id);
            return Results.BadRequest("Некорректный ID комнаты. ID должен быть положительным числом.");
        }
        var room = await db.Rooms
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.RoomType,
                Area = r.Info != null ? r.Info.Area : 0, // Если Info нет, возвращаем 0
                Temperature = r.Info != null ? r.Info.Temperature : 0,
                LightState = r.Info != null ? r.Info.LightState : false
            })
            .FirstOrDefaultAsync();

        if (room == null)
        {
            logger.LogWarning("Комната с ID {Id} не найдена.", id);
            return Results.NotFound($"Комната с ID {id} не найдена.");
        }

        logger.LogInformation("Данные для комнаты {Id} успешно получены.", id);
        return Results.Ok(room);
    }

    public static async Task<IResult> TurnLight(int id, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке переключить свет в комнате. ID: {Id}", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        var info = await db.RoomInfos.FirstOrDefaultAsync(i => i.RoomId == id);
        if (info == null)
        {
            logger.LogWarning("Информация для комнаты с ID {Id} не найдена.", id);
            return Results.NotFound($"Информация для комнаты с ID {id} не найдена.");
        }

        await Task.Delay(500); // Сетевая задержка
        info.LightState = !info.LightState;
        await db.SaveChangesAsync();

        logger.LogInformation("Состояние света для комнаты {Id} успешно переключено.", id);

        OutUpdateSensorDto outInfo = new OutUpdateSensorDto
        {
            Id = info.RoomId,
            Area = info.Area,
            Temperature = info.Temperature,
            LightState = info.LightState
        };

        return Results.Ok(outInfo);

    }

    // broken
    public static async Task<IResult> UpdateRoomSensors(int id, InUpdateSensorDto updateDto, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке обновить сенсоры комнаты. Данные: {@UpdateDto}", updateDto);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        var info = await db.RoomInfos.FirstOrDefaultAsync(i => i.RoomId == id);
        if (info == null)
        {
            logger.LogWarning("Информация для комнаты с ID {Id} не найдена.", id);
            return Results.NotFound($"Информация для комнаты с ID {id} не найдена.");
        }


        // Обновляем только те поля, которые были переданы в DTO
        if (updateDto.Area.HasValue)
            info.Area = updateDto.Area.Value;
        if (updateDto.Temperature.HasValue)
            info.Temperature = updateDto.Temperature.Value;
        if (updateDto.LightState.HasValue)
            info.LightState = updateDto.LightState.Value;

        await db.SaveChangesAsync();

        OutUpdateSensorDto updRoom = new OutUpdateSensorDto
        {
            Id = info.RoomId,
            Area = info.Area,
            Temperature = info.Temperature,
            LightState = info.LightState
        };

        logger.LogInformation("Информация для комнаты {Id} успешно обновлена.", id);
        return Results.Ok(updRoom);
    }


    public static async Task<IResult> UpdateRoomMetadata(int id, InUpdateRoomMetadataDto updateDto, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке обновить метаданные комнаты. Данные: {@UpdateDto}", updateDto);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        var room = await db.Rooms.FindAsync(id);
        if (room == null)
        {
            logger.LogWarning("Комната с ID {Id} не найдена для обновления.", id);
            return Results.NotFound($"Комната с ID {id} не найдена.");
        }

        // Обновляем только те поля, которые были переданы в DTO
        if (!string.IsNullOrWhiteSpace(updateDto.Name))
            room.Name = updateDto.Name;
        if (!string.IsNullOrWhiteSpace(updateDto.Description))
            room.Description = updateDto.Description;
        if (!string.IsNullOrWhiteSpace(updateDto.RoomType))
            room.RoomType = updateDto.RoomType;

        await db.SaveChangesAsync();

        OutUpdateRoomMetadataDto updRoom = new OutUpdateRoomMetadataDto
        {
            Id = room.Id,
            Name = room.Name,
            Description = room.Description,
            RoomType = room.RoomType
        };

        logger.LogInformation("Комната с ID {Id} успешно обновлена.", id);
        return Results.Ok(updRoom);
    }
    public static async Task<IResult> DeleteRoom(int id, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Сетевая ошибка при попытке удалить комнату с ID {Id}.", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)

        if (id < 0)
        {
            logger.LogError("Некорректный ID комнаты: {Id}. ID должен быть положительным числом.", id);
            return Results.BadRequest("Некорректный ID комнаты. ID должен быть положительным числом.");
        }

        var room = await db.Rooms.FindAsync(id);
        if (room == null)
        {
            logger.LogWarning("Комната с ID {Id} не найдена для удаления.", id);
            return Results.NotFound($"Комната с ID {id} не найдена.");
        }

        db.Rooms.Remove(room);
        await db.SaveChangesAsync();

        logger.LogInformation("Комната с ID {Id} успешно удалена.", id);
        return Results.Ok();

    }


    public static async Task<IResult> CreateRoom(CreateRoomDto createDto, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке создать комнату. Данные: {@CreateDto}", createDto);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        // Валидация входных данных
        if (string.IsNullOrWhiteSpace(createDto.Name))
            return Results.BadRequest("Название комнаты обязательно.");
        if (string.IsNullOrWhiteSpace(createDto.Description))
            return Results.BadRequest("Описание комнаты обязательно.");
        if (string.IsNullOrWhiteSpace(createDto.RoomType))
            return Results.BadRequest("Тип комнаты обязателен.");
        if (createDto.Area <= 0)
            return Results.BadRequest("Площадь комнаты должна быть положительным числом.");
        if (createDto.Temperature < -50 || createDto.Temperature > 50)
            return Results.BadRequest("Температура должна быть в диапазоне от -50 до 50 градусов Цельсия.");

        // Проверяем существование квартиры
        var apartmentExists = await db.Apartments.AnyAsync(a => a.Id == createDto.ApartmentId);
        if (!apartmentExists)
            return Results.BadRequest($"Квартира с ID {createDto.ApartmentId} не найдена.");

        // Создаем новую комнату
        var newRoom = new Room
        {
            Name = createDto.Name,
            Description = createDto.Description,
            RoomType = createDto.RoomType,
            ApartmentId = createDto.ApartmentId,
            Info = new RoomInfo
            {
                Area = createDto.Area,
                Temperature = createDto.Temperature,
                LightState = createDto.LightState
            }
        };

        db.Rooms.Add(newRoom);
        await db.SaveChangesAsync();

        FlatRoomDto flatRoomDto = new FlatRoomDto
        {
            Id = newRoom.Id,
            Name = newRoom.Name,
            Description = newRoom.Description,
            RoomType = newRoom.RoomType,
            ApartmentId = newRoom.ApartmentId
        };

        logger.LogInformation("Комната '{Name}' успешно создана с ID {Id}.", newRoom.Name, newRoom.Id);
        return Results.Created($"/api/rooms/{newRoom.Id}", flatRoomDto);
    }

    #endregion


}


public static partial class RoomEndpoints
{
    #region Endpoint mappings

    public static void MapRoomEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/rooms").WithTags("Rooms");

        group.MapGet("/", GetAllRooms)
            .WithSummary("Список комнат")
            .WithDescription("Получить список всех комнат во всех квартирах. Название, описание, тип комнаты и Id квартиры.")
            .Produces<List<FlatRoomDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", CreateRoom)
                    .WithSummary("Создать комнату")
                    .WithDescription("Создать новую комнату в квартире. Необходимо указать название, описание, тип комнаты и ID квартиры.")
                    .Produces<FlatRoomDto>(StatusCodes.Status201Created)
                    .Produces(StatusCodes.Status400BadRequest)
                    .Produces(StatusCodes.Status500InternalServerError)
                    .Accepts<CreateRoomDto>("application/json")
                    ;


        group.MapGet("/{id}", GetRoomInfo)
            .WithSummary("Датчики комнаты")
            .WithDescription("Получить информацию о комнате по её ID (НЕ номеру квартиры). Включает в себя площадь, температуру, состояние света, название и описание.")
            .Produces<RoomDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);



        group.MapPatch("/{id}", UpdateRoomMetadata)
        .WithSummary("Обновить комнату")
        .WithDescription("Обновить название, описание и тип комнаты по её ID (НЕ номеру квартиры).")
        .Produces<OutUpdateRoomMetadataDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .Accepts<InUpdateRoomMetadataDto>("application/json");

        group.MapPatch("/{id}/sensors", UpdateRoomSensors)
                    .WithSummary("Обновить информацию о комнате")
                    .WithDescription("Обновить информацию о комнате по её ID (НЕ номеру квартиры). Можно обновлять площадь, температуру и состояние света.")
                    .Produces<OutUpdateSensorDto>(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                    .Accepts<InUpdateSensorDto>("application/json")
                    ;

        group.MapPatch("/{id}/light", TurnLight)
            .WithSummary("Переключить свет в комнате")
            .WithDescription("Переключить состояние света в комнате по её ID (НЕ номеру квартиры).")
            .Produces<OutUpdateSensorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            ;




        group.MapDelete("/{id}", DeleteRoom)
            .WithSummary("Удалить комнату")
            .WithDescription("Удалить комнату по её ID (НЕ номеру квартиры).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    #endregion




}