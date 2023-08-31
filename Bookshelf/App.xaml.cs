using BLL;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.Views;

namespace Bookshelf;

public partial class App : Application
{

    //to do
    //criação de componentes
    //utilizar item tapped nas list views na seleção
    //ajuste da nomeclatura dos itens de acordo com o padrao que criei
    //passar as chamadas de api p uma camada repo propria
    //improvement: criar timeline

    //<Nome do objeto><ação da interface>
    //list - listagem
    //upt - inclusão ou update
    //del - exclusão
    //display - visualização
    //insert - somente inclusão
    //update - somente update

    //criar um model result para os retornos de respostas

    public App(ISyncServices syncServices, IUserBLL userBLL, IBuildDbBLL buildDbBLL)
    {
        try
        {
            //talvez implementar o versionamento do banco p rodar o clean no init em caso de necessidade?
            //Task.Run(buildDbBLL.CleanLocalDatabase).Wait();

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
