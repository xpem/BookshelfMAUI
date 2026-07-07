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

    public readonly string Version = "@0.2.5";

    private ISyncService SyncServices { get; set; }

    private IUserService UserBLL { get; set; }

    private IBuildDbService BuildDbService { get; set; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var appShellVM = new AppShellVM(SyncServices, BuildDbService, UserBLL);

        BuildDbService.Init();

        _ = appShellVM.AtualizaUserShowData();

        User user = UserBLL.GetUserLocal().Result;

        if (user != null)
        {
            Uid = user.Id;
            SyncServices.StartThread();
        }

        return new Window(new AppShell(appShellVM));
    }

    public App(ISyncService syncServices, IUserService userBLL, IBuildDbService buildDbService)
    {
        RegisterCrashHandlers();

        SyncServices = syncServices;
        UserBLL = userBLL;
        BuildDbService = buildDbService;

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
