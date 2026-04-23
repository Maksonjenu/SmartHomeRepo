using SmartHomeRepo.Entitys;
using System.Linq;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        // 1. ПРОВЕРКА: Если в базе уже есть квартиры, ничего не делаем.
        if (db.Apartments.Any()) return;

        // 2. СОЗДАЕМ КВАРТИРЫ
        var apt1 = new Apartment { Number = "101", Description = "Люкс в центре" };
        var apt2 = new Apartment { Number = "42-Б", Description = "Бюджетная студия" };

        db.Apartments.AddRange(apt1, apt2);
        db.SaveChanges(); // Сохраняем, чтобы получить ID для связей

        // 3. СОЗДАЕМ КОМНАТЫ И ИНФО (Сразу связываем объекты)
        var rooms = new List<Room>
        {
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Гостиная", 
                RoomType = "LivingRoom", 
                Description = "Большая светлая комната",
                Info = new RoomInfo { Area = 25.5, Temperature = 22.0, LightState = true }
            },
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Кухня", 
                RoomType = "Kitchen", 
                Description = "Современная кухня",
                Info = new RoomInfo { Area = 12.0, Temperature = 24.5, LightState = false }
            },
            new Room { 
                ApartmentId = apt2.Id, 
                Name = "Студия", 
                RoomType = "Studio", 
                Description = "Всё в одном",
                Info = new RoomInfo { Area = 18.0, Temperature = 21.0, LightState = true }
            }
        };

        db.Rooms.AddRange(rooms);
        db.SaveChanges();
    }
}