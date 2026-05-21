using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class CrearObjetivoPage : ContentPage
{
    private readonly CrearObjetivoViewModel _viewModel;

    public CrearObjetivoPage(CrearObjetivoViewModel viewModel)
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