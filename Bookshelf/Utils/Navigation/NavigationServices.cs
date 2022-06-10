using Bookshelf.ViewModels.Components;
using System.Diagnostics;

namespace Bookshelf.Utils.Navigation
{
    public class NavigationServices : INavigationServices
    {
        protected INavigationServices navigation;
        protected IServiceProvider services;

        public NavigationServices(IServiceProvider _services) => services = _services;

        public T ResolvePage<T>() where T : Page => services.GetService<T>();

        protected static INavigation Navigation
        {
            get
            {
                INavigation navigation = Application.Current?.MainPage?.Navigation;

                if (navigation is not null)
                    return navigation;
                else
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    throw new Exception();
                }
            }
        }

        public Task NavigateBack()
        {
            if (Navigation.NavigationStack.Count > 1)
                return Navigation.PopAsync();

            throw new InvalidOperationException("No pages to navigate back to!");
        }

    }
}
