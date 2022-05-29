namespace BookshelfServices.User
{
    public interface IUserServices
    {
        Task<BookshelfModels.User.User?> InsertUser(string email, string password);

        BookshelfModels.User.User? GetUserLocal();

        Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User? user);
    }
}
