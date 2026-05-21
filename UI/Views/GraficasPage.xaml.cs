using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class GraficasPage : ContentPage
{
    private readonly GraficasViewModel _viewModel;

    public GraficasPage(GraficasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Solo lo cargamos si no hay datos todavía para evitar los duplicados
        if (_viewModel.DatosCategorias.Count == 0)
            _viewModel.CargarGraficasCommand.Execute(null);
    }
}