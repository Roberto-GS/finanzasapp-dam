using FinanzasApp.UI.ViewModels;

namespace FinanzasApp.UI.Views;

public partial class DetalleGrupoPage : ContentPage
{
    public DetalleGrupoPage(DetalleGrupoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}