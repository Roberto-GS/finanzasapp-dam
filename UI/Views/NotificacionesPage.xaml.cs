using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class NotificacionesPage : ContentPage
{
    private readonly NotificacionesViewModel _viewModel;
    private bool _yaInicializado;

    public NotificacionesPage(NotificacionesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_yaInicializado) return;
        _yaInicializado = true;
        _viewModel.CargarCommand.Execute(null);
    }
}