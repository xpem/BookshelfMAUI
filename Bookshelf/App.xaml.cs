using Bookshelf.Views;
using BLL.Books.Sync;
using BLL.User;

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

    public App(IBooksSyncBLL booksSyncServices)
    {
        BLL.BuildDbBLL.BuildSQLiteDb();

        InitializeComponent();

        if (UserBLL.GetUserLocal().Result != null)
        {
            booksSyncServices.StartThread();

            MainPage = new AppShell();
            Shell.Current.GoToAsync($"//{nameof(Main)}");
        }
        else
        {
            MainPage = new AppShell();
        }
    }
}
