using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class DetalleObjetivoPage : ContentPage
{
    private readonly DetalleObjetivoViewModel _viewModel;

    public DetalleObjetivoPage(DetalleObjetivoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Recargamos cada vez que se muestra la página por si hubo algún cambio
        if (_viewModel.ObjetivoId > 0)
            _viewModel.CargarDatosCommand.Execute(null);
    }
}