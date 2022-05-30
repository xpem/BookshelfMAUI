namespace BookshelfServices.User.AuthServices
{
    public interface IUserAuthServices
    {
        Task<(BookshelfModels.User.User?, bool)> RefreshUserToken(BookshelfModels.User.User? user);

        Task<BookshelfModels.User.User?> SignInWithEmailAndPassword(string? email, string? password);

        Task<BookshelfModels.User.User?> CreateUser(string email, string password);
    }
}
