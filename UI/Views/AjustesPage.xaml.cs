using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class AjustesPage : ContentPage
{
    private readonly AjustesViewModel _viewModel;

    public AjustesPage(AjustesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarDatosCommand.Execute(null);
    }
}