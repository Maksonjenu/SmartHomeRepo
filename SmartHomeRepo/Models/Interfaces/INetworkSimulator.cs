namespace SmartHomeRepo.Models.Interfaces;

public interface INetworkSimulator
{
    // Может вернуть ошибку (500/503), а может null (значит всё ок, работаем дальше)
    Task<IResult?> TryGetRandomErrorAsync();

    // Позволяет модифицировать данные перед отправкой (занулить поля или очистить список)
    List<T> MessWithData<T>(List<T> data);

}