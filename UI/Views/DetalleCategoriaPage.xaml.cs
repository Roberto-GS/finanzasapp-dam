using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class DetalleCategoriaPage : ContentPage
{
    public DetalleCategoriaPage(DetalleCategoriaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}