using System.Net.Http;
using System.Net.Http.Json;
using SmartView.Models;
using SmartView.Models.DTOs;

namespace SmartView.Services;

/// <summary>
/// Упрощённый API сервис для работы с SmartHome API
/// Используется для обучения студентов
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseAddress;

    public ApiService(string? baseAddress = null)
    {
        _baseAddress = baseAddress ?? "http://localhost:5000";
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseAddress),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    /// <summary>
    /// Проверяет соединение с сервером
    /// </summary>
    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/apartments");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Получает список всех квартир
    /// </summary>
    public async Task<List<ApartmentDto>> GetApartmentsAsync()
    {
        try
        {
            var apartments = await _httpClient.GetFromJsonAsync<List<ApartmentDto>>("/api/apartments");
            
            if (apartments == null)
                throw new Exception("API вернул пустой ответ");

            return apartments;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при получении квартир: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Получает список комнат для квартиры по ID
    /// </summary>
    public async Task<List<RoomModel>> GetRoomsByApartmentAsync(int apartmentId)
    {
        try
        {
            var rooms = await _httpClient.GetFromJsonAsync<List<RoomModel>>($"/api/apartments/{apartmentId}/rooms");
            
            if (rooms == null)
                throw new Exception("API вернул пустой ответ");

            return rooms;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при получении комнат: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Получает информацию о конкретной комнате
    /// </summary>
    public async Task<RoomModel> GetRoomInfoAsync(int roomId)
    {
        try
        {
            var room = await _httpClient.GetFromJsonAsync<RoomModel>($"/api/rooms/{roomId}");
            
            if (room == null)
                throw new Exception("Комната не найдена");

            return room;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при получении информации о комнате: {ex.Message}", ex);
        }
    }
}
