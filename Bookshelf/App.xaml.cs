using Bookshelf.Services.Sync;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.User;

namespace Bookshelf;

public partial class App : Application
{
    public int Uid { get; set; }

    public App(ISyncService syncServices, IUserService userBLL, IBuildDbService buildDbBLL)
    {
        try
        {
            buildDbBLL.Init();

            InitializeComponent();

            User user = userBLL.GetUserLocal().Result;

            MainPage = new AppShell(new ViewModels.AppShellVM(user, syncServices, buildDbBLL, userBLL));

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
