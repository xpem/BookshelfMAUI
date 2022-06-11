using BookshelfRepos.User;
using BookshelfServices.User.AuthServices;

namespace BookshelfServices.User
{
    public class UserServices : IUserServices
    {
        private readonly IUserAuthServices userAuthServices;

        public UserServices(IUserAuthServices _userAuthServices)
        {
            userAuthServices = _userAuthServices;
        }

        /// <summary>
        /// get user in the static var
        /// </summary>
        /// <returns></returns>
        public BookshelfModels.User.User? GetUserLocal() => UserRepos.GetUser();

        public async Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User? user)
        {
            (BookshelfModels.User.User? userResponse, bool Success) = await userAuthServices.RefreshUserToken(user);

            if (Success && userResponse is not null)
            {
                UserRepos.UpdateToken(userResponse.Id, userResponse.Token);
            }

            return userResponse;
        }

        /// <summary>
        /// Get user in fb and add him in sqlite
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string email, string password)
        {
            try
            {
                BookshelfModels.User.User? user = await userAuthServices.SignInWithEmailAndPassword(email, password);

                if (user is not null)
                {
                    if (user.Error != null)
                        return false;
                    else
                    {
                        UserRepos.InsertUser(user);
                        return true;
                    }
                }
                else
                    return false;
            }
            catch { throw; }
        }

        public async Task<BookshelfModels.User.User> InsertUser(string email, string password)
        {
            BookshelfModels.User.User? user = await userAuthServices.CreateUser(email, password);

            if (user != null)
            {
                if (user.Error != null)
                {
                    return user;
                }
                else
                {
                    BookshelfRepos.User.UserRepos.InsertUser(user);
                    return user;
                }
            }
            else
                return null;
        }

        public static void CleanUserDatabase() => UserRepos.CleanUserDatabase();
    }
}
