using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.User;

namespace Bookshelf;

public partial class App : Application
{
    public int? Uid { get; set; }

    public readonly string Version = "@0.2.5";

    private ISyncService SyncServices { get; set; }

    private IUserService UserBLL { get; set; }

    private IBuildDbService BuildDbBLL { get; set; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var appShellVM = new AppShellVM(SyncServices, BuildDbBLL, UserBLL);

        BuildDbBLL.Init();

        _ = appShellVM.AtualizaUserShowData();

        User user = UserBLL.GetUserLocal().Result;

        if (user != null)
        {
            Uid = user.Id;
            SyncServices.StartThread();
        }

        return new Window(new AppShell(appShellVM));
    }

    public App(ISyncService syncServices, IUserService userBLL, IBuildDbService buildDbBLL)
    {
        SyncServices = syncServices;
        UserBLL = userBLL;
        BuildDbBLL = buildDbBLL;

        InitializeComponent();
    }
}
