using SmartHomeRepo.Models.Interfaces;

public class RealNetwork : INetworkSimulator
{
    private readonly Random _rng = new();
    private readonly ILogger<RealNetwork> _logger;

    public RealNetwork(ILogger<RealNetwork> logger) => _logger = logger;

    public async Task<IResult?> TryGetRandomErrorAsync()
    {
        double roll = _rng.NextDouble();

        // 5% Экстремальная задержка
        if (roll < 0.05) {
            _logger.LogCritical("CHAOS: Экстремальная задержка 45 сек!");
            await Task.Delay(45_000); 
        }
        // 10% Ошибка сервера
        else if (roll < 0.15) {
            _logger.LogError("CHAOS: Рандомная 500 ошибка!");
            return Results.StatusCode(500);
        }
        
        // В остальных случаях - обычная задержка 1-5 сек для вида
        await Task.Delay(_rng.Next(500, 5000));
        return null; 
    }

    public List<T> MessWithData<T>(List<T> data)
    {
        double roll = _rng.NextDouble();

        // 3% Пустой список
        if (roll < 0.03) {
            _logger.LogWarning("CHAOS: Очистка списка данных!");
            return new List<T>();
        }

        // 10% Зануление полей ( Defensive programming check )
        if (roll < 0.13) {
            _logger.LogWarning("CHAOS: Порча данных (null-инъекция)!");
            // Тут можно через рефлексию занулить случайное строковое поле у половины объектов
            foreach (var item in data.Take(data.Count / 2)) {
                var props = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
                foreach (var p in props) p.SetValue(item, null);
            }
        }

        return data;
    }
}