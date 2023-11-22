using BLL.Books.Historic.Interfaces;
using DbContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL : IBookHistoricBLL
    {
        private readonly IBookHistoricDAL bookHistoricDAL;
        private readonly IUserDAL userDAL;

        public BookHistoricBLL(IBookHistoricDAL bookHistoricDAL, IUserDAL userDAL)
        {
            this.bookHistoricDAL = bookHistoricDAL;
            this.userDAL = userDAL;
        }

        public BookHistoricList GetBookHistoricByBookId(int? page, int bookId)
        {
            int uid = userDAL.GetUid();

            int pageSize = 10;

            List<BookHistoric> list = bookHistoricDAL.GetBookHistoricByBookId(uid, bookId);

            int total = list.Count;
            if (page != null)
                list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();

            return new BookHistoricList() { List = list, Total = total };
        }
    }
}
