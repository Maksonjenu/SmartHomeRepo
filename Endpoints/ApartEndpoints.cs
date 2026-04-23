using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;

namespace SmartHomeRepo.Endpoints;

public static partial class ApartmentEndpoints
{


    private static async Task<IResult> GetAllApartments(AppDbContext db)
    {
        var apartments = await db.Apartments.Select(a => new ApartmentDto
        {
            Id = a.Id,
            Number = a.Number,
            Description = a.Description,
            RoomsCount = a.Rooms.Count
        }).ToListAsync();

        logger.LogInformation("Получено {Count} квартир из базы данных.", apartments.Count);
        logger.LogInformation("Отправляю список квартир клиенту.");

        return Results.Ok(apartments); // ОБЯЗАТЕЛЬНО возвращаем данные
    }

    private static async Task<IResult> GetRoomsInApartment([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db)
    {
        var apartment = await db.Apartments
            .Include(a => a.Rooms)
                .ThenInclude(r => r.Info)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (apartment == null)
        {
            logger.LogWarning("Квартира с ID {Id} не найдена.", id);
            return Results.NotFound($"Квартира с ID {id} не найдена.");
        }

        var rooms = apartment.Rooms.Select(r => new
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            RoomType = r.RoomType,
            Area = r.Info?.Area ?? 0, // Если Info нет, возвращаем
            Temperature = r.Info?.Temperature ?? 0,
            LightState = r.Info?.LightState ?? false


        }).ToList();


        //  await db.Apartments
        //      .Include(a => a.Rooms)          // Уровень 1: Загружаем список комнат
        //          .ThenInclude(r => r.Info)   // Уровень 2: Для КАЖДОЙ комнаты загружаем её Info
        //      .FirstOrDefaultAsync(a => a.Id == id)
        //          is { } apt ? return Results.Ok(apt.Rooms) : Results.NotFound()


        logger.LogInformation("Получено {Count} комнат для квартиры ID {Id}.", rooms.Count, id);
        return Results.Ok(rooms);
    }

    private static async Task<IResult> CreateApartment([Description("Объект для создания квартиры, внутри строки номера и названия квартиры.")] CreateApartmentRequest req, AppDbContext db)
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

        // 4. Ответ (возвращаем созданный объект с его новым ID)
        return Results.Created($"/api/apartments/{newApartment.Id}", newApartment);
    }
}


public static partial class ApartmentEndpoints
{
    static ILogger logger;

    public static void MapApartmentEndpoints(this IEndpointRouteBuilder routes, ILogger _logger)
    {

        logger = _logger;

        var group = routes.MapGroup("/api/apartments").WithTags("Apartments");

        group.MapGet("/", GetAllApartments)
            .WithSummary("Список всех квартир")
            .WithDescription("Получить список всех квартир.")
            .Produces<List<ApartmentDto>>(StatusCodes.Status200OK); // Вот эта магия!
        ;

        group.MapGet("/{id}/rooms", GetRoomsInApartment)
     .WithSummary("Комнаты в квартире со всеми датчиками")
     .WithDescription("Получить список всех комнат в квартире по её ID. ВНИМАНИЕ: ответ содержит ВСЕ датчики внутри каждой комнаты!")
     .Produces<List<RoomDto>>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status404NotFound)
     ;


        group.MapPost("/create", CreateApartment)
    .WithSummary("Создать новую квартиру")
    .WithDescription("Создает новую квартиру. В теле запроса должен быть JSON с полями 'number' (строка) и 'description' (строка, необязательно).")
    .Produces<Apartment>(StatusCodes.Status201Created) // Возвращаем созданную квартиру с её ID
    .Produces(StatusCodes.Status400BadRequest) // Если номер квартиры не указан
    ;


        group.MapDelete("/{id}/delete", async ([Description("ID квартиры (НЕ номер квартиры)")] int id, AppDbContext db) =>
        {
            var apt = await db.Apartments.FindAsync(id);
            if (apt == null)
                return Results.NotFound();

            db.Apartments.Remove(apt);
            await db.SaveChangesAsync();
            return Results.NoContent();

        })
        .WithSummary("Удалить квартиру по ID")
        .WithDescription("Удаляет квартиру по её ID. ВНИМАНИЕ: удаление квартиры удалит ВСЕ комнаты и датчики внутри неё!");
    }


    public class RoomDto
    {
        [Description("ID комнаты")]
        public int Id { get; set; }
        [Description("Название комнаты")]
        public string Name { get; set; }
        [Description("Описание комнаты")]
        public string Description { get; set; }

        [Description("Тип комнаты")]
        public string RoomType { get; set; }
        [Description("Площадь комнаты в квадратных метрах")]
        public double Area { get; set; }
        [Description("Температура в комнате в градусах Цельсия")]
        public double Temperature { get; set; }
        [Description("Состояние света в комнате (true - включен, false - выключен)")]
        public bool LightState { get; set; }

    }

    public class ApartmentDto
    {
        [Description("ID квартиры (НЕ номер квартиры)")]
        public int Id { get; set; }
        [Description("Номер квартиры")]
        public string Number { get; set; }
        [Description("Описание квартиры")]
        public string Description { get; set; }

        [Description("Количество комнат")]
        public int RoomsCount { get; set; }
    }

    public class CreateApartmentRequest
    {
        [Description("Номер квартиры")]
        public string Number { get; set; }

        [Description("Описание квартиры")]
        public string Description { get; set; }
    }
}