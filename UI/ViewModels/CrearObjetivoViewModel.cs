using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.Core.DTOs.Objetivo;
using FinanzasApp.Core.Enums;
using FinanzasApp.UI.Services;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

[QueryProperty(nameof(ObjetivoId), "ObjetivoId")]
public partial class CrearObjetivoViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _objetivoId;

    [ObservableProperty]
    private bool _modoEdicion;

    [ObservableProperty]
    private string _tituloPagina = "Nuevo objetivo";

    [ObservableProperty]
    private string _textoBoton = "Crear objetivo";

    [ObservableProperty]
    private string _descripcion = string.Empty;

    [ObservableProperty]
    private string _cantidadObjetivo = string.Empty;

    [ObservableProperty]
    private TipoObjetivoDto? _tipoSeleccionado;

    [ObservableProperty]
    private CategoriaDto? _categoriaSeleccionada;

    [ObservableProperty]
    private DateTime _fechaInicio = DateTime.Today;

    [ObservableProperty]
    private DateTime _fechaFin = DateTime.Today.AddMonths(1);

    // Visibilidad de campos según el tipo

    [ObservableProperty]
    private bool _mostrarCategoria;

    [ObservableProperty]
    private bool _categoriaObligatoria;

    [ObservableProperty]
    private string _textoAyuda = string.Empty;

    public ObservableCollection<TipoObjetivoDto> TiposObjetivo { get; } = [];
    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    public CrearObjetivoViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Nuevo objetivo";
    }

    partial void OnObjetivoIdChanged(int value)
    {
        if (value > 0)
        {
            ModoEdicion = true;
            TituloPagina = "Editar objetivo";
            TextoBoton = "Guardar cambios";
            CargarDatosCommand.Execute(null);
        }
    }

    partial void OnTipoSeleccionadoChanged(TipoObjetivoDto? value)
    {
        // Actualizamos la visibilidad y los textos de ayuda al cambiar el tipo
        ActualizarCamposPorTipo(value);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        await EjecutarSeguro(async () =>
        {
            Task<List<TipoObjetivoDto>> tiposTask = _apiService.ObtenerTiposObjetivo();
            Task<List<CategoriaDto>> categoriasTask = _apiService.ObtenerCategorias();

            await Task.WhenAll(tiposTask, categoriasTask);

            TiposObjetivo.Clear();
            foreach (TipoObjetivoDto t in await tiposTask)
                TiposObjetivo.Add(t);

            Categorias.Clear();
            foreach (CategoriaDto c in await categoriasTask)
                Categorias.Add(c);

            if (ModoEdicion && ObjetivoId > 0)
                await CargarDatosObjetivoAsync();
            else if (TiposObjetivo.Any())
                TipoSeleccionado = TiposObjetivo.First();

        }, "Error al cargar los datos");
    }

    private async Task CargarDatosObjetivoAsync()
    {
        ObjetivoConProgresoDto objetivo =
            await _apiService.ObtenerObjetivoConProgreso(ObjetivoId);

        Descripcion = objetivo.Descripcion;
        CantidadObjetivo = objetivo.CantidadObjetivo?.ToString("F2") ?? string.Empty;
        FechaInicio = objetivo.FechaInicio ?? DateTime.Today;
        FechaFin = objetivo.FechaFin ?? DateTime.Today.AddMonths(1);
        TipoSeleccionado = TiposObjetivo.FirstOrDefault(t => t.Id == objetivo.TipoId);

        if (objetivo.CategoriaId.HasValue)
            CategoriaSeleccionada = Categorias
                .FirstOrDefault(c => c.Id == objetivo.CategoriaId.Value);
    }

    /// <summary>
    /// Actualiza qué campos se muestran y el texto de ayuda
    /// en función del tipo de objetivo seleccionado.
    /// </summary>
    private void ActualizarCamposPorTipo(TipoObjetivoDto? tipo)
    {
        if (tipo is null) return;

        switch (tipo.Id)
        {
            case 1: // Ahorro
                MostrarCategoria = false;
                CategoriaObligatoria = false;
                CategoriaSeleccionada = null;
                TextoAyuda = "El ahorro mide tu balance (ingresos - gastos)" +
                             " en el periodo. No filtra por categoría";
                break;

            case 2: // Límite de gasto
                MostrarCategoria = true;
                CategoriaObligatoria = false;
                TextoAyuda = "Controla que tus gastos no superen el límite en el periodo" +
                             " Puedes limitarlo a una categoría concreta o a todos tus gastos";
                break;

            case 3: // Meta de ingreso
                MostrarCategoria = false;
                CategoriaObligatoria = false;
                CategoriaSeleccionada = null;
                TextoAyuda = "Controla que tus ingresos totales alcancen " +
                             " la cantidad objetivo en el periodo";
                break;

            case 4: // Reducción de deuda
                MostrarCategoria = true;
                CategoriaObligatoria = true;
                TextoAyuda = "Controla que los gastos de una categoría concreta " +
                             " se mantengan por debajo del límite. La categoría es obligatoria";
                break;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (TipoSeleccionado is null)
        {
            MostrarError("Selecciona un tipo de objetivo");
            return;
        }

        // Validación: Reducción de deuda requiere categoría
        if (TipoSeleccionado.Id == 4 && CategoriaSeleccionada is null)
        {
            MostrarError("La reducción de deuda requiere seleccionar una categoría");
            return;
        }

        if (FechaFin < FechaInicio)
        {
            MostrarError("La fecha fin no puede ser anterior a la fecha de inicio");
            return;
        }

        if (string.IsNullOrWhiteSpace(CantidadObjetivo))
        {
            MostrarError("La cantidad objetivo es obligatoria");
            return;
        }

        if (!decimal.TryParse(CantidadObjetivo, out decimal cantidad) || cantidad <= 0)
        {
            MostrarError("Introduce una cantidad válida mayor que 0");
            return;
        }

        await EjecutarSeguro(async () =>
        {
            CrearObjetivoDto dto = new CrearObjetivoDto
            {
                TipoId = TipoSeleccionado.Id,
                // Solo enviamos categoría si el tipo la admite
                CategoriaId = MostrarCategoria ? CategoriaSeleccionada?.Id : null,
                CantidadObjetivo = cantidad,
                // El periodo se fija a Mensual, la duración real viene de las fechas
                Periodo = PeriodoObjetivo.Mensual,
                Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            if (ModoEdicion)
                await _apiService.EditarObjetivo(ObjetivoId, dto);
            else
                await _apiService.CrearObjetivo(dto);

            await Shell.Current.GoToAsync("..");

        }, ModoEdicion ? "Error al guardar los cambios" : "Error al crear el objetivo");
    }

    [RelayCommand]
    private async Task CancelarAsync()
        => await Shell.Current.GoToAsync("..");
}