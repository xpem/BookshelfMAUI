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

            MainPage = new AppShell();

            if (user != null)
            {
                syncServices.StartThread();
                Shell.Current.GoToAsync($"//{nameof(Main)}");
            }
        }
        catch (Exception ex) { throw ex; }
    }
}
