using Bookshelf.Services.Sync;
using Bookshelf.ViewModels;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.User;

namespace Bookshelf;

public partial class App : Application
{
    public int Uid { get; set; }

    public readonly string Version = "@0.1.5";

    public App(ISyncService syncServices, IUserService userBLL, IBuildDbService buildDbBLL)
    {
        try
        {
            buildDbBLL.Init();

            InitializeComponent();

            User user = userBLL.GetUserLocal().Result;

            var appShellVM = new AppShellVM(syncServices, buildDbBLL, userBLL);

            MainPage = new AppShell(appShellVM);

            appShellVM.AtualizaUser();

            if (user != null)
            {
                Uid = user.Id;
                syncServices.StartThread();
                Shell.Current.GoToAsync($"//{nameof(Main)}");
            }
        }
        catch (Exception ex) { throw; }
    }
}
