using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class MovimientosPage : ContentPage
{
    private readonly MovimientosViewModel _viewModel;

    public MovimientosPage(MovimientosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarMovimientosCommand.Execute(null);
    }
}