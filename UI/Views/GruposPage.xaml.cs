using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class GruposPage : ContentPage
{
    private readonly GruposViewModel _viewModel;

    public GruposPage(GruposViewModel viewModel)
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