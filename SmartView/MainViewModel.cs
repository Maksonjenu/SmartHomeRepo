using SmartHome.Core.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
// Твои DTO тоже должны быть доступны тут

namespace SmartView
{
    public class MainViewModel
    {
        private readonly HttpClient _http;

        // Конструктор просит фабрику клиентов, а контейнер из App.xaml.cs её дает
        public MainViewModel(IHttpClientFactory httpClientFactory)
        {
            // Берем тот самый настроенный клиент "ApiClient"
            _http = httpClientFactory.CreateClient("ApiClient");
        }

        // --- Метод 1: Получить все квартиры ---
        public async Task<List<ApartmentDto>> LoadApartmentsAsync()
        {
            try
            {
                // GetFromJsonAsync сам делает запрос и десериализует JSON в список
                var apartments = await _http.GetFromJsonAsync<List<ApartmentDto>>("apartments");

                // Если пришел null, возвращаем пустой список, чтобы не словить NullReferenceException
                return apartments ?? new List<ApartmentDto>();
            }
            catch (Exception ex)
            {
                // Тут можно вывести ошибку в UI (например, через MessageBox)
                Console.WriteLine($"Ошибка загрузки квартир: {ex.Message}");
                return new List<ApartmentDto>();
            }
        }

        // --- Метод 2: Получить комнаты для конкретной квартиры (с датчиками) ---
        public async Task<List<RoomDto>> LoadRoomsForApartmentAsync(int apartmentId)
        {
            try
            {
                var rooms = await _http.GetFromJsonAsync<List<RoomDto>>($"apartments/{apartmentId}/rooms");
                return rooms ?? new List<RoomDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки комнат: {ex.Message}");
                return new List<RoomDto>();
            }
        }

        // --- Метод 3 (Опционально): Получить вообще все комнаты (плоские) ---
        public async Task<List<FlatRoomDto>> LoadAllRoomsAsync()
        {
            try
            {
                var rooms = await _http.GetFromJsonAsync<List<FlatRoomDto>>("rooms");
                return rooms ?? new List<FlatRoomDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки всех комнат: {ex.Message}");
                return new List<FlatRoomDto>();
            }
        }
    }
}
