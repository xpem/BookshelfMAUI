using BookshelfModels.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookshelfServices.Books
{
    public class BooksServices : IBooksServices
    {
        public BookshelfModels.User.User? User { get; set; }

        public BooksServices()
        {
            User = BookshelfRepos.User.UserRepos.GetUser();
        }

        public async Task<Totals> GetBookshelfTotals()
        {
            Totals BTotals = new();

            if (User?.Id != null)
            {
                List<(Situation, int)> list = await BookshelfRepos.Books.BooksRepos.GetBookshelfTotals(User.Id);

                if (list.Count > 0)
                {
                    BTotals.IllRead = list.Where(a => a.Item1 == Situation.IllRead).FirstOrDefault().Item2;
                    BTotals.Reading = list.Where(a => a.Item1 == Situation.Reading).FirstOrDefault().Item2;
                    BTotals.Read = list.Where(a => a.Item1 == Situation.Read).FirstOrDefault().Item2;
                    BTotals.Interrupted = list.Where(a => a.Item1 == Situation.Interrupted).FirstOrDefault().Item2;
                }
                else
                {
                    BTotals.IllRead = BTotals.Reading = BTotals.Read = BTotals.Interrupted = 0;
                }
            }

            return BTotals;

        }
    }
}
