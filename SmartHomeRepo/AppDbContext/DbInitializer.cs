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
        var apt3 = new Apartment { Number = "29", Description = "Магазин на первом этаже" };
        

        db.Apartments.AddRange(apt1, apt2, apt3);
        db.SaveChanges(); // Сохраняем, чтобы получить ID для связей

        // 3. СОЗДАЕМ КОМНАТЫ И ИНФО (Сразу связываем объекты)
        var rooms = new List<Room>
        {
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Гостиная", 
                RoomType = "LivingRoom", 
                Description = "Большая светлая комната (без окон) с большим удобным диваном",
                Info = new RoomInfo { Area = 2, Temperature = 22.0, LightState = true }
            },
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Спальня", 
                RoomType = "Bedroom", 
                Description = "Уютная комната для сна собаки",
                Info = new RoomInfo { Area = 15.0, Temperature = 20.0, LightState = false }
            },
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Ванная", 
                RoomType = "Bathroom", 
                Description = "Современная ванная комната, только почему-то унитаз прикручен к потолку",
                Info = new RoomInfo { Area = 8.0, Temperature = 24.0, LightState = false }
            },
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Кухня", 
                RoomType = "Kitchen", 
                Description = "Современная кухня на которой вас ждет горячая пицца флорентина",
                Info = new RoomInfo { Area = 12.0, Temperature = 24.5, LightState = false }
            },
            new Room { 
                ApartmentId = apt1.Id, 
                Name = "Комната для пылесоса", 
                RoomType = "Storage", 
                Description = "Небольшое кладовое помещение для семейного робота-пылесоса по имени Отец Пигидий",
                Info = new RoomInfo { Area = 150.0, Temperature = 19.0, LightState = false }
            },
            new Room { 
                ApartmentId = apt2.Id, 
                Name = "Студия", 
                RoomType = "Studio", 
                Description = "Всё в одном, туалет совмещен с раковиной, а кровать выступает в роли рабочего места и гладильной доски",
                Info = new RoomInfo { Area = 18.0, Temperature = 21.0, LightState = true }
            },
            new Room { 
                ApartmentId = apt3.Id, 
                Name = "Зона продаж", 
                RoomType = "Commerce", 
                Description = "Десять касс из которых работают только две",
                Info = new RoomInfo { Area = 60.0, Temperature = 20.0, LightState = true }
            },
             new Room { 
                ApartmentId = apt3.Id, 
                Name = "Склад", 
                RoomType = "Storage", 
                Description = "Со склада иногда подозрительно теряется товар",
                Info = new RoomInfo { Area = 80.0, Temperature = 15.0, LightState = false }
            },
            new Room { 
                ApartmentId = apt3.Id, 
                Name = "Холодильники", 
                RoomType = "Commerce", 
                Description = "Молочка, мясо, рыба, полуфабрикаты и замерзшая мышь",
                Info = new RoomInfo { Area = 70.0, Temperature = 10.0, LightState = false }
            },
            new Room { 
                ApartmentId = apt3.Id, 
                Name = "Комната директора", 
                RoomType = "Office", 
                Description = "В этом месте находятся товары которые пропали со склада",
                Info = new RoomInfo { Area = 15.0, Temperature = 18.0, LightState = false }
            },




            

        };

        db.Rooms.AddRange(rooms);
        db.SaveChanges();
    }
}