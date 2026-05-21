using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class ObjetivosPage : ContentPage
{
    private readonly ObjetivosViewModel _viewModel;

    public ObjetivosPage(ObjetivosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarObjetivosCommand.Execute(null);
    }
}