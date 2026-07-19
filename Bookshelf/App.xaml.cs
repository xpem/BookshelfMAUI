using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.User;

namespace Bookshelf;

public partial class App : Application
{
    public static string CrashLogPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "crash.log");

    public int? Uid { get; set; }

    public readonly string Version = "@0.3.5";

    private ISyncService _syncService { get; set; }

    private IUserService _userService { get; set; }

    public IUserSessionService _userSessionService { get; set; }

    private IBuildDbService _buildDbService { get; set; }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Exibe loading enquanto inicializa
        var loadingPage = new ContentPage
        {
            Content = new ActivityIndicator { IsRunning = true, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
        };

        var window = new Window(loadingPage);

        _ = InitializeAndNavigateAsync(window);

        return window;
    }

    private async Task InitializeAndNavigateAsync(Window window)
    {
        await _buildDbService.Init();
        //await _userService.GetMockUserAsync();
        //await _accountService.MockAccount(1);
        //await _categoryService.MockCategories(1);

        var appShellVM = new AppShellVM(_syncService, _buildDbService, _userService, _userSessionService);
        await appShellVM.UserFlyoutAsync();

        // Se o usuário já está logado, inicia o sync em background
        var user = await _userSessionService.GetCurrentUserAsync();
        if (user != null)
            _syncService.StartThread();

        // Só navega para o Shell após tudo pronto
        window.Page = new AppShell(appShellVM);
    }

    public App(ISyncService syncServices, IUserService userService, IBuildDbService buildDbService, IUserSessionService userSessionService)
    {
        RegisterCrashHandlers();

        _syncService = syncServices;
        _userService = userService;
        _buildDbService = buildDbService;
        _userSessionService = userSessionService;
        InitializeComponent();
    }

    private static void WriteCrashLog(string source, string content)
    {
        try
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}]{Environment.NewLine}{content}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}";
            File.AppendAllText(CrashLogPath, entry);
        }
        catch { /* n�o pode crashar o crash handler */ }
    }

    private static void RegisterCrashHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            WriteCrashLog("UnhandledException", ex?.ToString() ?? args.ExceptionObject?.ToString() ?? "unknown");
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            WriteCrashLog("UnobservedTaskException", args.Exception?.ToString() ?? "unknown");
            args.SetObserved(); // evita que o processo seja encerrado
        };
    }

}
