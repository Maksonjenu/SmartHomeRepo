using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartView.Models;
using SmartView.Services;

namespace SmartView.ViewModels;

/// <summary>
/// ViewModel для экрана списка комнат в квартире
/// </summary>
public class RoomsViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly MainViewModel _mainViewModel;
    private readonly int _apartmentId;
    private ObservableCollection<RoomModel> _rooms = new();
    private bool _isLoading = true;
    private string? _errorMessage;
    private string _apartmentTitle = "";
    private RoomModel? _selectedRoom;

    public RoomsViewModel(ApiService apiService, MainViewModel mainViewModel, int apartmentId, string apartmentNumber)
    {
        _apiService = apiService;
        _mainViewModel = mainViewModel;
        _apartmentId = apartmentId;
        ApartmentTitle = $"Квартира #{apartmentNumber}";

        GoBackCommand = new RelayCommand(_ => GoBack());
        SelectRoomCommand = new RelayCommand(
            _ => SelectRoom(),
            _ => SelectedRoom != null
        );

        LoadRooms();
    }

    public ObservableCollection<RoomModel> Rooms
    {
        get => _rooms;
        set => SetProperty(ref _rooms, value);
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

    public string ApartmentTitle
    {
        get => _apartmentTitle;
        set => SetProperty(ref _apartmentTitle, value);
    }

    public RoomModel? SelectedRoom
    {
        get => _selectedRoom;
        set => SetProperty(ref _selectedRoom, value);
    }

    public ICommand GoBackCommand { get; }
    public ICommand SelectRoomCommand { get; }

    private async void LoadRooms()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var rooms = await _apiService.GetRoomsByApartmentAsync(_apartmentId);
            Rooms = new ObservableCollection<RoomModel>(rooms);

            IsLoading = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке комнат: {ex.Message}";
            IsLoading = false;
        }
    }

    private void GoBack()
    {
        _mainViewModel.NavigateToApartments();
    }

    private void SelectRoom()
    {
        if (SelectedRoom != null)
        {
            _mainViewModel.NavigateToRoomDetails(SelectedRoom.Id);
        }
    }
}
