using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;
using SmartHomeRepo.DTO;
using SmartHomeRepo.Models.Interfaces;

namespace SmartHomeRepo.Endpoints;

public class LogCategory
{
    public const string Name = "ApartmentEndpoints";
}

public static partial class ApartmentEndpoints
{

    #region Handlers

    private static async Task<IResult> GetAllApartments(AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        logger.LogInformation("Пользователь отправил запрос на получение списка всех квартир.");
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке получить список квартир.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        if (db.Apartments == null)
        {
            logger.LogError("Ошибка доступа к базе данных: таблица Apartments не найдена.");
            return Results.Problem("Внутренняя ошибка сервера: таблица Apartments не найдена.");
        }

        var apartments = await db.Apartments.Select(a => new ApartmentDto
        {
            Id = a.Id,
            Number = a.Number,
            Description = a.Description,
            RoomsCount = a.Rooms.Count
        }).ToListAsync();

        logger.LogInformation("Получено {Count} квартир из базы данных.", apartments.Count);
        logger.LogInformation("Отправляю список квартир клиенту.");


        apartments = network.MessWithData(apartments); // Имитируем порчу данных (может очистить список или занулить поля)

        logger.LogInformation("Отправляю список квартир клиенту.");
        return Results.Ok(apartments); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

    private static async Task<IResult> GetRoomsInApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        logger.LogInformation("Пользователь отправил запрос на получение списка комнат для квартиры с ID {Id}.", id);
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке получить список комнат для квартиры. ID: {Id}", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (id < 0)
        {
            logger.LogError("Некорректный ID квартиры: {Id}. ID должен быть положительным числом.", id);
            return Results.BadRequest("Некорректный ID квартиры. ID должен быть положительным числом.");
        }

        var apartmentExists = await db.Apartments.AnyAsync(a => a.Id == id);
        if (!apartmentExists)
        {
            logger.LogWarning("Квартира с ID {Id} не найдена.", id);
            return Results.NotFound($"Квартира с ID {id} не найдена.");
        }

        var rooms = await db.Rooms
            .Where(r => r.ApartmentId == id)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                RoomType = r.RoomType,
                Area = r.Info != null ? r.Info.Area : 0, // Если Info нет, возвращаем 0
                Temperature = r.Info != null ? r.Info.Temperature : 0,
                LightState = r.Info != null ? r.Info.LightState : false
            })
            .ToListAsync();

        //  await db.Apartments
        //      .Include(a => a.Rooms)          // Уровень 1: Загружаем список комнат
        //          .ThenInclude(r => r.Info)   // Уровень 2: Для КАЖДОЙ комнаты загружаем её Info
        //      .FirstOrDefaultAsync(a => a.Id == id)
        //          is { } apt ? return Results.Ok(apt.Rooms) : Results.NotFound()


        rooms = network.MessWithData(rooms); // Имитируем порчу данных (может очистить список или занулить поля)

        logger.LogInformation("Получено {Count} комнат для квартиры ID {Id}.", rooms.Count, id);
        logger.LogInformation("Отправляю список комнат для квартиры ID {Id} клиенту.", id);        

        return Results.Ok(rooms);
    }

    private static async Task<IResult> CreateApartment([Description("Объект для создания квартиры, внутри строки номера и названия квартиры.")] WriteApartmentDto req, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        logger.LogInformation("Пользователь отправил запрос на создание новой квартиры.");
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке создать квартиру. Данные: {@CreateDto}", req);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        // 1. Валидация (минимально, чтобы GUI не слал мусор)
        if (string.IsNullOrWhiteSpace(req.Number))
        {
            logger.LogError("Номер квартиры обязателен.");
            return Results.BadRequest("Номер квартиры обязателен");
        }

        //2. Проверяем уникальность номера квартиры (хотя бы на уровне БД, но тут для примера)
        if (await db.Apartments.AnyAsync(a => a.Number == req.Number))

            {
                logger.LogError("Квартира с номером {Number} уже существует.", req.Number);
                return Results.BadRequest("Квартира с таким номером уже существует");
            }

        // 2. Маппинг: Создаем сущность БД из "дефолтных типов" DTO
        var newApartment = new Apartment
        {
            Number = req.Number,
            Description = req.Description
        };

        // 3. Сохранение
        db.Apartments.Add(newApartment);
        await db.SaveChangesAsync();

        logger.LogInformation("Создана новая квартира с ID {Id} и номером {Number}.", newApartment.Id, newApartment.Number);

        ApartmentDto apartmentDto = new ApartmentDto
        {
            Id = newApartment.Id,
            Number = newApartment.Number,
            Description = newApartment.Description,
            RoomsCount = 0 // Новая квартира, комнат пока нет
        };

        logger.LogInformation("Отправляю информацию о новой квартире клиенту {Id}.", apartmentDto.Id);
        apartmentDto = network.MessWithData(new List<ApartmentDto> { apartmentDto }).First(); // Имитируем порчу данных (может занулить поля)

        // 4. Ответ (возвращаем созданный объект с его новым ID)
        return Results.Created($"/api/apartments/{newApartment.Id}", apartmentDto);
    }

    private static async Task<IResult> DeleteApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {

        logger.LogInformation("Пользователь отправил запрос на удаление квартиры с ID {Id}.", id);
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке удалить квартиру. ID: {Id}", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        var apt = await db.Apartments.FindAsync(id);
        if (apt == null)
        {
            logger.LogWarning("Попытка удалить квартиру с ID {Id}, но она не найдена.", id);
            return Results.NotFound();
        }

        logger.LogWarning("Удаляю квартиру ID {Id} с номером {Number}.", apt.Id, apt.Number);
        db.Apartments.Remove(apt);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, WriteApartmentDto req, AppDbContext db, ILogger<LogCategory> logger, INetworkSimulator network)
    {
        logger.LogInformation("Пользователь отправил запрос на обновление информации о квартире с ID {Id}.", id);
        if (await network.TryGetRandomErrorAsync() == Results.StatusCode(StatusCodes.Status500InternalServerError)) // Имитируем нестабильную сеть (может добавить задержку или вернуть ошибку)
        {
            logger.LogError("Внутренняя ошибка при попытке обновить квартиру. ID: {Id}", id);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        // Попытка найти квартиру по ID
        var apartment = await db.Apartments.FindAsync(id);
        if (apartment == null)
        {
            logger.LogWarning("Попытка обновить квартиру с ID {Id}, но она не найдена.", id);
            return Results.NotFound($"Квартира с ID {id} не найдена.");
        }

        if (req == null)
        {
            logger.LogError("Некорректные данные запроса.");
            return Results.BadRequest("Некорректные данные запроса.");
        }

        if (string.IsNullOrWhiteSpace(req.Number))
        {
            logger.LogError("Номер квартиры обязателен.");
            return Results.BadRequest("Номер квартиры обязателен");
        }

        // Проверка уникальности номера квартиры (кроме текущей)
        if (await db.Apartments.AnyAsync(a => a.Number == req.Number && a.Id != id))
        {
            logger.LogError("Квартира с номером {Number} уже существует.", req.Number);
            return Results.BadRequest("Квартира с таким номером уже существует");
        }

        // Обновление свойств
        apartment.Number = req.Number;
        apartment.Description = req.Description;

        await db.SaveChangesAsync();
        logger.LogInformation("Обновлена информация о квартире ID {Id}.", id);
        logger.LogInformation("Отправляю обновленную информацию о квартире ID {Id} клиенту.", id);

        ApartmentDto apartmentDto = new ApartmentDto
        {
            Id = apartment.Id,
            Number = apartment.Number,
            Description = apartment.Description,
            RoomsCount = apartment.Rooms.Count
        };

        return Results.Ok(apartmentDto);
    }

    #endregion

}






public static partial class ApartmentEndpoints
{

    #region Endpoint mappings

    public static void MapApartmentEndpoints(this IEndpointRouteBuilder routes)
    {

        var group = routes.MapGroup("/api/apartments").WithTags("Apartments");

        // 1. Сначала получение списка (самый легкий и частый запрос)
        group.MapGet("/", GetAllApartments)
            .WithSummary("Список всех квартир")
            .WithDescription("Получить список всех квартир. В ответе возвращается массив объектов, каждый из которых содержит ID, номер, описание и количество комнат в квартире.")
            .Produces<List<ApartmentDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            ;

        // 2. Создание (обычно идет сразу после GET списка в документации)
        group.MapPost("/", CreateApartment)
            .WithSummary("Создать новую квартиру")
            .WithDescription("Создать новую квартиру с указанным номером и описанием. Номер квартиры должен быть уникальным. В ответе возвращается созданная квартира с её новым ID.")
            .Accepts<WriteApartmentDto>("application/json") // Указываем, что ожидаем JSON с полями для создания квартиры
            .Produces<ApartmentDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            ;

        // 3. Обновление и удаление конкретной квартиры (группируем по {id})
        group.MapPut("/{id}", UpdateApartment)
            .WithSummary("Обновить информацию о квартире")
            .WithDescription("Обновить информацию о квартире по ее ID. Если квартира с указанным ID не найдена, возвращается 404 Not Found.")
            .Accepts<WriteApartmentDto>("application/json") // Указываем, что ожидаем JSON с полями для обновления квартиры
            .Produces<ApartmentDto>(StatusCodes.Status200OK) // ЗАМЕТЬ: тут заменил на Dto
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            ;

        group.MapDelete("/{id}", DeleteApartment)
            .WithSummary("Удалить квартиру по ID")
            .WithDescription("Удалить квартиру по ее ID. Если квартира с указанным ID не найдена, возвращается 404 Not Found.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            ;

        // 4. Вложенные ресурсы (идем вглубь иерархии)
        group.MapGet("/{id}/rooms", GetRoomsInApartment)
            .WithSummary("Комнаты в квартире со всеми датчиками")
            .WithDescription("Получить список всех комнат в квартире по ID квартиры. В ответе возвращается подробная информация о каждой комнате, включая площадь, температуру и состояние света. Если квартира с указанным ID не найдена, возвращается 404 Not Found.")
            .Produces<List<RoomDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            ;
    }

    #endregion



}