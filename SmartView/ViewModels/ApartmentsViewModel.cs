using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartView.Models;
using SmartView.Models.DTOs;
using SmartView.Services;

namespace SmartView.ViewModels;

/// <summary>
/// ViewModel для экрана списка квартир
/// </summary>
public class ApartmentsViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly MainViewModel _mainViewModel;
    private ObservableCollection<ApartmentDto> _apartments = new();
    private bool _isLoading = true;
    private string? _errorMessage;
    private ApartmentDto? _selectedApartment;

    public ApartmentsViewModel(ApiService apiService, MainViewModel mainViewModel)
    {
        _apiService = apiService;
        _mainViewModel = mainViewModel;

        SelectApartmentCommand = new RelayCommand(
            _ => SelectApartment(),
            _ => SelectedApartment != null
        );

        LoadApartments();
    }

    public ObservableCollection<ApartmentDto> Apartments
    {
        get => _apartments;
        set => SetProperty(ref _apartments, value);
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

    public ApartmentDto? SelectedApartment
    {
        get => _selectedApartment;
        set => SetProperty(ref _selectedApartment, value);
    }

    public ICommand SelectApartmentCommand { get; }

    private async void LoadApartments()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var apartments = await _apiService.GetApartmentsAsync();
            Apartments = new ObservableCollection<ApartmentDto>(apartments);

            IsLoading = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке квартир: {ex.Message}";
            IsLoading = false;
        }
    }

    private void SelectApartment()
    {
        if (SelectedApartment != null)
        {
            _mainViewModel.NavigateToRooms(SelectedApartment.Id, SelectedApartment.Number);
        }
    }
}
