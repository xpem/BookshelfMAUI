using BookshelfModels.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookshelfServices.Books
{
    public interface IBooksServices
    {
        Task<Totals> GetBookshelfTotals();
    }
}
