using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class DetalleMovimientoPage : ContentPage
{
    private readonly DetalleMovimientoViewModel _viewModel;

    public DetalleMovimientoPage(DetalleMovimientoViewModel viewModel)
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