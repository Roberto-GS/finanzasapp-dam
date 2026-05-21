using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Core.DTOs.Categoria;
using FinanzasApp.UI.Services;
using FinanzasApp.UI.Views;
using System.Collections.ObjectModel;

namespace FinanzasApp.UI.ViewModels;

/// <summary>
/// ViewModel de la página de categorías. Gestiona la lista,
/// creación, eliminación y navegación al detalle de cada categoría.
/// </summary>
public partial class CategoriasViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<CategoriaDto> Categorias { get; } = [];

    public CategoriasViewModel(
        IApiService apiService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Titulo = "Categorías";
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
    private async Task CrearCategoriaAsync()
        => await _navigationService.Navegar(nameof(CrearCategoriaPage));

    [RelayCommand]
    private async Task EliminarCategoriaAsync(CategoriaDto categoria)
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar categoría",
            $"¿Seguro que quieres eliminar '{categoria.Nombre}'? " +
            "Los movimientos asociados pasarán a 'Sin categoría'",
            "Eliminar", "Cancelar");

        if (!confirmar) return;

        await EjecutarSeguro(async () =>
        {
            await _apiService.EliminarCategoria(categoria.Id);
            Categorias.Remove(categoria);
        }, "Error al eliminar la categoría");
    }

    [RelayCommand]
    private async Task VerDetalleCategoriaAsync(CategoriaDto categoria)
        => await _navigationService.Navegar(nameof(DetalleCategoriaPage),
            new Dictionary<string, object> { { "Categoria", categoria } });
}