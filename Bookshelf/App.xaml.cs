using BLL;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.Views;

namespace Bookshelf;

public partial class App : Application
{
    public App(ISyncServices syncServices, IUserBLL userBLL, IBuildDbBLL buildDbBLL)
    {
        try
        {
            Task.Run(buildDbBLL.Init).Wait();

            InitializeComponent();

            Models.User user = userBLL.GetUserLocal().Result;

            if (user != null)
            {
                syncServices.StartThread();

                MainPage = new AppShell();
                Shell.Current.GoToAsync($"//{nameof(Main)}");
            }
            else
            {
                MainPage = new AppShell();
            }
        }
        catch (Exception ex) { throw ex; }
    }
}
