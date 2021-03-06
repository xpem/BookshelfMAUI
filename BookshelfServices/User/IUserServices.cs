namespace BookshelfServices.User
{
    public interface IUserServices
    {
        BookshelfModels.User.User? GetUserLocal();

        Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User? user);

        Task<bool> SignIn(string email, string password);

        Task<BookshelfModels.User.User> InsertUser(string email, string password);
    }
}
