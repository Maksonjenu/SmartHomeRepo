using System.Windows.Controls;
using System.Windows.Input;
using SmartView.Services;
using SmartView.Views;

namespace SmartView.ViewModels;

/// <summary>
/// Главный ViewModel для управления навигацией и страницами приложения
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private Page? _currentPage;
    private BaseViewModel? _currentViewModel;
    private int _lastApartmentId = 0;
    private string _lastApartmentNumber = "";

    public MainViewModel()
    {
        _apiService = new ApiService();
        NavigateToConnectionCommand = new RelayCommand(_ => NavigateToConnection());
        
        // Загружаем первую страницу (проверка соединения)
        NavigateToConnection();
    }

    /// <summary>
    /// Текущая отображаемая страница
    /// </summary>
    public Page? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    /// <summary>
    /// Текущий ViewModel
    /// </summary>
    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand NavigateToConnectionCommand { get; }

    // Методы навигации

    private void NavigateToConnection()
    {
        var viewModel = new ConnectionViewModel(_apiService, this);
        CurrentViewModel = viewModel;
        CurrentPage = new ConnectionPage { DataContext = viewModel };
    }

    public void NavigateToApartments()
    {
        var viewModel = new ApartmentsViewModel(_apiService, this);
        CurrentViewModel = viewModel;
        CurrentPage = new ApartmentsPage { DataContext = viewModel };
    }

    public void NavigateToRooms(int apartmentId, string apartmentNumber)
    {
        _lastApartmentId = apartmentId;
        _lastApartmentNumber = apartmentNumber;
        
        var viewModel = new RoomsViewModel(_apiService, this, apartmentId, apartmentNumber);
        CurrentViewModel = viewModel;
        CurrentPage = new RoomsPage { DataContext = viewModel };
    }

    public void NavigateToRoomDetails(int roomId)
    {
        var viewModel = new RoomDetailsViewModel(_apiService, this, roomId, _lastApartmentId);
        CurrentViewModel = viewModel;
        CurrentPage = new RoomDetailsPage { DataContext = viewModel };
    }
}
