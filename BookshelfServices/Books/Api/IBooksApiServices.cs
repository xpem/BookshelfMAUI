using BookshelfModels.Books;

namespace BookshelfServices.Books.Api
{
    public interface IBooksApiServices
    {
        Task<(bool, string)> AddBook(Book book, BookshelfModels.User.User? user);

        Task<List<Book>?> GetBooksByLastUpdate(BookshelfModels.User.User? user);
    }
}
