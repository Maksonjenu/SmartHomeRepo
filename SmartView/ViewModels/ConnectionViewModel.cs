using System.Windows.Input;
using SmartView.Services;

namespace SmartView.ViewModels;

/// <summary>
/// ViewModel для экрана проверки соединения с сервером
/// </summary>
public class ConnectionViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly MainViewModel _mainViewModel;
    private string _statusMessage = "Проверка соединения...";
    private bool _isLoading = true;
    private bool _isError = false;
    private bool _isConnected = false;

    public ConnectionViewModel(ApiService apiService, MainViewModel mainViewModel)
    {
        _apiService = apiService;
        _mainViewModel = mainViewModel;

        RetryCommand = new RelayCommand(_ => CheckConnection(), _ => IsError);

        // Проверяем соединение при инициализации
        CheckConnection();
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }

    public ICommand RetryCommand { get; }

    private async void CheckConnection()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            StatusMessage = "Проверка соединения...";

            var result = await _apiService.CheckConnectionAsync();

            if (result)
            {
                IsConnected = true;
                StatusMessage = "✓ Соединение установлено!";
                IsLoading = false;

                // Автоматически переходим на список квартир через 1 секунду
                await Task.Delay(1000);
                _mainViewModel.NavigateToApartments();
            }
            else
            {
                IsError = true;
                StatusMessage = "✗ Не удаётся подключиться к серверу";
                IsLoading = false;
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"✗ Ошибка: {ex.Message}";
            IsLoading = false;
        }
    }
}
