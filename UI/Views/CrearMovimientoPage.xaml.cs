using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class CrearMovimientoPage : ContentPage
{
    private readonly CrearMovimientoViewModel _viewModel;

    public CrearMovimientoPage(CrearMovimientoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarCategoriasCommand.Execute(null);
    }
}