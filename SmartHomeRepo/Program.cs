using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Endpoints;
using SmartHomeRepo.Models.Interfaces;

// 1. ПАРСИНГ АРГУМЕНТОВ
// Мы можем передавать: --urls "http://0.0.0.0:5000" или свои параметры типа --db "my.db"
var builder = WebApplication.CreateBuilder(args);

bool enablePranks = args.Contains("--enable-network") || 
                    builder.Configuration.GetValue<bool>("EnablePranks");

if (enablePranks)
{
    builder.Services.AddSingleton<INetworkSimulator, RealNetwork>();
}
else
{
    builder.Services.AddSingleton<INetworkSimulator, DebugNetwork>();
}



// Настройка порта и адреса через аргументы командной строки
// Если запустить: dotnet run --urls "http://localhost:8080"
// Или через кастомный аргумент --port (код ниже):
var port = builder.Configuration["port"] ?? "5000"; // По умолчанию 5197
var host = builder.Configuration["host"] ?? "0.0.0.0"; // По умолчанию localhost
builder.WebHost.UseUrls($"http://{host}:{port}");

// Настройка пути к БД через аргументы: dotnet run --db "production.db"
var dbPath = builder.Configuration["db"] ?? "smart_home.db";

// Подключаем SQLite с динамическим именем файла
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite($"Data Source={dbPath}"));

// РЕГИСТРАЦИЯ СЕРВИСОВ
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath)) 
        options.IncludeXmlComments(xmlPath);
});

builder.Logging.AddSimpleConsole(options => 
{
    options.TimestampFormat = "[HH:mm:ss] ";
});

var app = builder.Build();

// Инициализация базы данных (делаем ОДИН раз)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = app.Logger;

    if (db.Database.EnsureCreated())
    {
        logger.LogWarning("База данных [{DbPath}] не найдена. Создаю новую...", dbPath);
        DbInitializer.Seed(db);
        logger.LogInformation("Начальные данные успешно загружены.");
    }
    else
    {
        logger.LogInformation("База данных [{DbPath}] уже существует.", dbPath);
        // Если нужно перетирать данные при каждом запуске (для тестов):
        // DbInitializer.Seed(db); 
    }
}

// Настройка Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

// МАППИНГ ЭНДПОИНТОВ
// Передаем app.Logger, если MapApartmentEndpoints его требует
app.MapApartmentEndpoints(); // Лучше доставать логгер внутри эндпоинта через DI
app.MapRoomEndpoints();

app.Logger.LogInformation("Сервер запущен на {Host}:{Port}", host, port);

app.Run();