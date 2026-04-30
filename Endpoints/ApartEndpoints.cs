using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;
using SmartHomeRepo.DTO;

namespace SmartHomeRepo.Endpoints;

public class LogCategory
{
    public const string Name = "ApartmentEndpoints";
}

public static partial class ApartmentEndpoints
{

    #region Handlers

    static Random random = new Random();

    private static async Task<IResult> GetAllApartments(AppDbContext db, ILogger<LogCategory> logger)
    {
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


        await Task.Delay(random.Next(0,10_000)); // Имитируем задержку для демонстрации логов


        return Results.Ok(apartments); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

    private static async Task<IResult> GetRoomsInApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db, ILogger<LogCategory> logger)
    {

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


        logger.LogInformation("Получено {Count} комнат для квартиры ID {Id}.", rooms.Count, id);
        return Results.Ok(rooms);
    }

    private static async Task<IResult> CreateApartment([Description("Объект для создания квартиры, внутри строки номера и названия квартиры.")] CreateApartmentRequest req, AppDbContext db, ILogger<LogCategory> logger)
    {
        // 1. Валидация (минимально, чтобы GUI не слал мусор)
        if (string.IsNullOrWhiteSpace(req.Number))
            return Results.BadRequest("Номер квартиры обязателен");

        //2. Проверяем уникальность номера квартиры (хотя бы на уровне БД, но тут для примера)
        if (await db.Apartments.AnyAsync(a => a.Number == req.Number))
            return Results.BadRequest("Квартира с таким номером уже существует");

        // 2. Маппинг: Создаем сущность БД из "дефолтных типов" DTO
        var newApartment = new Apartment
        {
            Number = req.Number,
            Description = req.Description
        };

        // 3. Сохранение
        db.Apartments.Add(newApartment);
        await db.SaveChangesAsync();

        ApartmentDto apartmentDto = new ApartmentDto
        {
            Id = newApartment.Id,
            Number = newApartment.Number,
            Description = newApartment.Description,
            RoomsCount = 0 // Новая квартира, комнат пока нет
        };

        // 4. Ответ (возвращаем созданный объект с его новым ID)
        return Results.Created($"/api/apartments/{newApartment.Id}", apartmentDto);
    }

    private static async Task<IResult> DeleteApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db, ILogger<LogCategory> logger)
    {
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

    private static async Task<IResult> UpdateApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, CreateApartmentRequest req, AppDbContext db, ILogger<LogCategory> logger)
    {
        // Попытка найти квартиру по ID
        var apartment = await db.Apartments.FindAsync(id);
        if (apartment == null)
        {
            logger.LogWarning("Попытка обновить квартиру с ID {Id}, но она не найдена.", id);
            return Results.NotFound($"Квартира с ID {id} не найдена.");
        }

        if (req == null)
            return Results.BadRequest("Некорректные данные запроса.");

        if (string.IsNullOrWhiteSpace(req.Number))
            return Results.BadRequest("Номер квартиры обязателен");

        // Проверка уникальности номера квартиры (кроме текущей)
        if (await db.Apartments.AnyAsync(a => a.Number == req.Number && a.Id != id))
            return Results.BadRequest("Квартира с таким номером уже существует");

        // Обновление свойств
        apartment.Number = req.Number;
        apartment.Description = req.Description;

        await db.SaveChangesAsync();
        logger.LogInformation("Обновлена информация о квартире ID {Id}.", id);
        return Results.Ok(apartment);
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
            .Produces<List<ApartmentDto>>(StatusCodes.Status200OK);

        // 2. Создание (обычно идет сразу после GET списка в документации)
        group.MapPost("/create", CreateApartment)
            .WithSummary("Создать новую квартиру")
            .Produces<ApartmentDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // 3. Обновление и удаление конкретной квартиры (группируем по {id})
        group.MapPut("/{id}", UpdateApartment)
            .WithSummary("Обновить информацию о квартире")
            .Produces<ApartmentDto>(StatusCodes.Status200OK) // ЗАМЕТЬ: тут заменил на Dto
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", DeleteApartment)
            .WithSummary("Удалить квартиру по ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // 4. Вложенные ресурсы (идем вглубь иерархии)
        group.MapGet("/{id}/rooms", GetRoomsInApartment)
            .WithSummary("Комнаты в квартире со всеми датчиками")
            .Produces<List<RoomDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    #endregion



}