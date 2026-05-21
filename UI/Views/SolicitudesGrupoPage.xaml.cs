using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class SolicitudesGrupoPage : ContentPage
{
    private readonly SolicitudesGrupoViewModel _viewModel;

    public SolicitudesGrupoPage(SolicitudesGrupoViewModel viewModel)
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