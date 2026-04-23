using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Подключаем SQLite
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite("Data Source=smart_home.db"));


    // 1. РЕГИСТРАЦИЯ СЕРВИСОВ (До builder.Build)
builder.Services.AddEndpointsApiExplorer(); // Нужно для поиска эндпоинтов
builder.Services.AddSwaggerGen();           // Генерирует саму спецификацию

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();


// Используем встроенный логгер приложения
var logger = app.Logger; 

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if(db.Database.EnsureCreated()) // Создает файл .db, если его нет
    {
        logger.LogWarning("База данных не найдена. Создаю новую и заполняю начальными данными...");
                
        
        DbInitializer.Seed(db);      // Заполняет данными
    }
    else
    {
        logger.LogWarning("База данных уже существует. Инициализация пропущена.");
                DbInitializer.Seed(db);      // Заполняет данными

    }
     
    
}


app.UseSwagger();   
app.UseSwaggerUI();

// АВТОМАТИЧЕСКОЕ СОЗДАНИЕ БАЗЫ (чтобы не мучить студентов миграциями)
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    // Тут можно вызвать метод Seed(db) для заполнения начальными данными
}

app.MapApartmentEndpoints(logger);
app.MapRoomEndpoints();

app.Run();