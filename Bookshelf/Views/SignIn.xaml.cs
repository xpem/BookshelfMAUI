using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class SignIn : ContentPage
{
    public SignIn(SignInVM loginVM)
    {
        InitializeComponent();

        BindingContext = loginVM;
    }

    private async void OnViewCrashLogTapped(object sender, TappedEventArgs e)
    {
        if (!File.Exists(App.CrashLogPath))
        {
            CrashLogLabel.IsVisible = false;
            return;
        }

        string log = await File.ReadAllTextAsync(App.CrashLogPath);

        bool clear = await DisplayAlert(
            "Log de erros",
            log.Length > 2000 ? "..." + log[^2000..] : log,
            "Limpar log",
            "Fechar");

        if (clear)
        {
            File.Delete(App.CrashLogPath);
            CrashLogLabel.IsVisible = false;
        }
    }
}