using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.UI.Services;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel para crear una nueva categoría. Gestiona el nombre,
/// la selección de color y el envío a la API.
/// </summary>
public partial class CrearCategoriaViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _color = "#2563EB";

    /// <summary>
    /// Paleta de colores disponibles para asignar a la categoría.
    /// </summary>
    public List<string> ColoresDisponibles { get; } =
    [
        "#2563EB", "#16A34A", "#DC2626", "#D97706",
        "#7C3AED", "#0891B2", "#DB2777", "#65A30D",
        "#EA580C", "#475569"
    ];

    public CrearCategoriaViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Nueva categoría";
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MostrarError("El nombre es obligatorio");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            CrearCategoriaDto dto = new CrearCategoriaDto
            {
                Nombre = Nombre,
                Color = Color
            };
            await _apiService.CrearCategoria(dto);
            await _navigationService.NavigarAtras();
        }, "Error al crear la categoría");
    }

    [RelayCommand]
    private async Task CancelarAsync()
        => await _navigationService.NavigarAtras();

    /// <summary>
    /// Actualiza el color seleccionado al pulsar un chip de color.
    /// </summary>
    [RelayCommand]
    private void SeleccionarColor(string color) => Color = color;
}