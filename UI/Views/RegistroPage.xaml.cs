using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(RegistroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}