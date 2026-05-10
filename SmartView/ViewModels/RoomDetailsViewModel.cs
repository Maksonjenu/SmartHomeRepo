using System.Windows.Input;
using SmartView.Models;
using SmartView.Services;

namespace SmartView.ViewModels;

/// <summary>
/// ViewModel для экрана с детальной информацией о комнате
/// </summary>
public class RoomDetailsViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly MainViewModel _mainViewModel;
    private readonly int _roomId;
    private readonly int _apartmentId;
    private RoomModel? _room;
    private bool _isLoading = true;
    private string? _errorMessage;

    public RoomDetailsViewModel(ApiService apiService, MainViewModel mainViewModel, int roomId, int apartmentId = 0)
    {
        _apiService = apiService;
        _mainViewModel = mainViewModel;
        _roomId = roomId;
        _apartmentId = apartmentId;

        GoBackCommand = new RelayCommand(_ => GoBack());

        LoadRoomInfo();
    }

    public RoomModel? Room
    {
        get => _room;
        set => SetProperty(ref _room, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand GoBackCommand { get; }

    private async void LoadRoomInfo()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var room = await _apiService.GetRoomInfoAsync(_roomId);
            Room = room;

            IsLoading = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке информации: {ex.Message}";
            IsLoading = false;
        }
    }

    private void GoBack()
    {
        // Возвращаемся на главный экран (список квартир)
        // Можно также вернуться на список комнат, если сохранить информацию о квартире
        _mainViewModel.NavigateToApartments();
    }
}
