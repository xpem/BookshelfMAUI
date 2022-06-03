using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshelf.Utils.Navigation
{
    public interface INavigationServices
    {
        T ResolvePage<T>() where T : Page;

        Task NavigateBack();
    }
}
