namespace LocalDbDAL.Books.BookHistoric
{
    public interface IBookHistoricLocalDAL
    {
        Task<bool> CheckBookHistoricById(int? id);

        Task AddBookHistoric(Models.Books.Historic.BookHistoric bookHistoric, int? userId);

        Task<List<Models.Books.Historic.BookHistoric>> GetBookHistoricByBookId(int uid, int bookId);
    }
}
