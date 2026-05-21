using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class CrearCategoriaPage : ContentPage
{
    public CrearCategoriaPage(CrearCategoriaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}