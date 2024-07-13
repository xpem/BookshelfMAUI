using BLL;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.Views;

namespace Bookshelf;

public partial class App : Application
{
    public int Uid { get; set; }

    public App(ISyncServices syncServices, IUserBLL userBLL, IBuildDbBLL buildDbBLL)
    {
        try
        {
            buildDbBLL.Init();

            InitializeComponent();

            Models.User user = userBLL.GetUserLocal().Result;

            MainPage = new AppShell(new ViewModels.AppShellVM(user, syncServices, buildDbBLL));

            if (user != null)
            {
                Uid = user.Id;
                syncServices.StartThread();
                Shell.Current.GoToAsync($"//{nameof(Main)}");
            }
        }
        catch (Exception ex) { throw ex; }
    }
}
