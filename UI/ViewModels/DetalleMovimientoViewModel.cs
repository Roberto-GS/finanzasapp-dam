using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Movimiento;
using FinanzasApp.Core.Enums;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel del detalle de un movimiento. Permite visualizar sus datos
/// y editarlos directamente desde la misma página.
/// </summary>
[QueryProperty(nameof(Movimiento), "Movimiento")]
public partial class DetalleMovimientoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private MovimientoDto? _movimiento;

    [ObservableProperty]
    private bool _modoEdicion;

    [ObservableProperty]
    private string _nombreEdicion = string.Empty;

    [ObservableProperty]
    private string _cantidadEdicion = string.Empty;

    [ObservableProperty]
    private string _descripcionEdicion = string.Empty;

    [ObservableProperty]
    private string _etiquetaEdicion = string.Empty;

    [ObservableProperty]
    private TipoMovimiento _tipoEdicion;

    [ObservableProperty]
    private CategoriaDto? _categoriaEdicion;

    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    public DetalleMovimientoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Detalle";
    }

    partial void OnMovimientoChanged(MovimientoDto? value)
    {
        // Al recibir el movimiento rellenamos los campos de edición
        if (value is null) return;
        NombreEdicion = value.Nombre;
        CantidadEdicion = value.Cantidad.ToString("F2");
        DescripcionEdicion = value.Descripcion ?? string.Empty;
        EtiquetaEdicion = value.Etiqueta ?? string.Empty;
        TipoEdicion = value.Tipo;
    }

    /// <summary>
    /// Carga las categorías disponibles y preselecciona
    /// la que tiene el movimiento si tiene una asignada.
    /// </summary>
    [RelayCommand]
    private async Task CargarCategoriasAsync()
    {
        await EjecutarSeguro(async () =>
        {
            List<CategoriaDto> categorias = await _apiService.ObtenerCategorias();
            Categorias.Clear();
            foreach (CategoriaDto c in categorias)
                Categorias.Add(c);

            // Preseleccionamos la categoría actual del movimiento
            if (Movimiento?.CategoriaId is not null)
                CategoriaEdicion = Categorias
                    .FirstOrDefault(c => c.Id == Movimiento.CategoriaId);
        }, "Error al cargar categorías");
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await _navigationService.NavigarAtras();

    /// <summary>Activa el modo edición para modificar los campos.</summary>
    [RelayCommand]
    private void ActivarEdicion() => ModoEdicion = true;

    /// <summary>
    /// Cancela la edición y restaura los valores originales del movimiento.
    /// </summary>
    [RelayCommand]
    private void CancelarEdicion()
    {
        ModoEdicion = false;
        if (Movimiento is null) return;

        NombreEdicion = Movimiento.Nombre;
        CantidadEdicion = Movimiento.Cantidad.ToString("F2");
        DescripcionEdicion = Movimiento.Descripcion ?? string.Empty;
        EtiquetaEdicion = Movimiento.Etiqueta ?? string.Empty;
        TipoEdicion = Movimiento.Tipo;
    }

    [RelayCommand]
    private async Task GuardarEdicionAsync()
    {
        if (Movimiento is null) return;

        if (string.IsNullOrWhiteSpace(NombreEdicion))
        {
            MostrarError("El nombre es obligatorio.");
            return;
        }

        if (!decimal.TryParse(CantidadEdicion, out decimal cantidad) || cantidad <= 0)
        {
            MostrarError("Introduce una cantidad válida mayor que 0.");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            EditarMovimientoDto dto = new EditarMovimientoDto
            {
                Nombre = NombreEdicion,
                Cantidad = cantidad,
                Tipo = TipoEdicion,
                CategoriaId = CategoriaEdicion?.Id,
                Descripcion = string.IsNullOrWhiteSpace(DescripcionEdicion)
                    ? null : DescripcionEdicion,
                Etiqueta = string.IsNullOrWhiteSpace(EtiquetaEdicion)
                    ? null : EtiquetaEdicion
            };

            await _apiService.EditarMovimiento(Movimiento.Id, dto);
            ModoEdicion = false;
            await _navigationService.NavigarAtras();
        }, "Error al guardar los cambios");
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (Movimiento is null) return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar movimiento",
            $"¿Seguro que quieres eliminar '{Movimiento.Nombre}'?",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        await EjecutarSeguro(async () =>
        {
            await _apiService.EliminarMovimiento(Movimiento.Id);
            await _navigationService.NavigarAtras();
        }, "Error al eliminar el movimiento");
    }
}