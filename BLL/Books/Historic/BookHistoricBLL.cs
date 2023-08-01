using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL : IBookHistoricBLL
    {

        LocalDbDAL.Books.BookHistoric.IBookHistoricLocalDAL BookHistoricLocalDAL;

        public BookHistoricBLL(LocalDbDAL.Books.BookHistoric.IBookHistoricLocalDAL bookHistoricLocalDAL)
        {
            BookHistoricLocalDAL = bookHistoricLocalDAL;
        }

        public async Task<BookHistoricList> GetBookHistoricByBookId(int? page, int bookId)
        {
            List<BookHistoric> list = new();

            int total = 0;

            Models.User? User = await LocalDbDAL.User.UserLocalDAL.GetUser();

            if (User?.Id != null)
            {
                int pageSize = 10;

                list = (await BookHistoricLocalDAL.GetBookHistoricByBookId(User.Id, bookId));

                total = list.Count;

                if (page != null)
                    list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();
            }

            return new BookHistoricList() { List = list, Total = total };
        }
    }
}
