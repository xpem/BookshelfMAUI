namespace Bookshelf.Utils.Navigation
{
    public interface INavigationServices
    {
        Task NavigateBack();

        Task NavigateToPage<T>(object parameter = null) where T : Page;
    }
}
