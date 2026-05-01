using SmartHomeRepo.Models.Interfaces;

public class DebugNetwork : INetworkSimulator
{
    private readonly Random _rng = new();
    private readonly ILogger<DebugNetwork> _logger;

    public DebugNetwork(ILogger<DebugNetwork> logger) => _logger = logger;

    public async Task<IResult?> TryGetRandomErrorAsync() => null; // Всегда всё ок, ошибок нет

    public List<T> MessWithData<T>(List<T> data)
    {
        return data;
    }
}