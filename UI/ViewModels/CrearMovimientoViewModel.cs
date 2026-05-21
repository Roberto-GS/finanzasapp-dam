using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.Enums;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel para registrar un nuevo movimiento económico.
/// Gestiona el formulario, la selección de tipo y categoría,
/// y el envío a la API.
/// </summary>
public partial class CrearMovimientoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _cantidad = string.Empty;

    [ObservableProperty]
    private string _descripcion = string.Empty;

    [ObservableProperty]
    private string _etiqueta = string.Empty;

    [ObservableProperty]
    private TipoMovimiento _tipoSeleccionado = TipoMovimiento.Gasto;

    [ObservableProperty]
    private CategoriaDto? _categoriaSeleccionada;

    [ObservableProperty]
    private bool _esGasto = true;

    [ObservableProperty]
    private bool _esIngreso;

    public ObservableCollection<CategoriaDto> Categorias { get; } = [];
    public List<TipoMovimiento> Tipos { get; } =
        [TipoMovimiento.Ingreso, TipoMovimiento.Gasto];

    public CrearMovimientoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Nuevo movimiento";
    }

    [RelayCommand]
    private async Task CargarCategoriasAsync()
    {
        await EjecutarSeguro(async () =>
        {
            List<CategoriaDto> categorias = await _apiService.ObtenerCategorias();
            Categorias.Clear();
            foreach (CategoriaDto c in categorias)
                Categorias.Add(c);
        }, "Error al cargar las categorías");
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MostrarError("El nombre es obligatorio");
            return;
        }

        if (!decimal.TryParse(Cantidad, out decimal cantidadDecimal) || cantidadDecimal <= 0)
        {
            MostrarError("Introduce una cantidad válida mayor que 0");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            CrearMovimientoDto dto = new CrearMovimientoDto
            {
                Nombre = Nombre,
                Cantidad = cantidadDecimal,
                Tipo = TipoSeleccionado,
                CategoriaId = CategoriaSeleccionada?.Id,
                Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion,
                Etiqueta = string.IsNullOrWhiteSpace(Etiqueta) ? null : Etiqueta
            };

            await _apiService.CrearMovimiento(dto);
            await _navigationService.NavigarAtras();
        }, "Error al guardar el movimiento");
    }

    [RelayCommand]
    private async Task CancelarAsync()
        => await _navigationService.NavigarAtras();

    /// <summary>Selecciona el tipo ingreso y actualiza los indicadores visuales.</summary>
    [RelayCommand]
    private void SeleccionarTipoIngreso()
    {
        TipoSeleccionado = TipoMovimiento.Ingreso;
        EsGasto = false;
        EsIngreso = true;
    }

    /// <summary>Selecciona el tipo gasto y actualiza los indicadores visuales.</summary>
    [RelayCommand]
    private void SeleccionarTipoGasto()
    {
        TipoSeleccionado = TipoMovimiento.Gasto;
        EsGasto = true;
        EsIngreso = false;
    }
}