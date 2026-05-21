using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class BuscarMovimientosPage : ContentPage
{
    private readonly BuscarMovimientosViewModel _viewModel;

    public BuscarMovimientosPage(BuscarMovimientosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarCommand.Execute(null);
    }
}