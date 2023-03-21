using BookshelfModels.User;
using BookshelfRepos.User;
using BookshelfServices.User.Api;

namespace BookshelfServices.User
{
    public class UserServices : IUserServices
    {
        /// <summary>
        /// get user in the static var
        /// </summary>
        /// <returns></returns>
        public BookshelfModels.User.User? GetUserLocal() => UserRepos.GetUser();

        public async Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User user)
        {
            try
            {
                BookshelfModels.User.User userResponse = user;

                if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Password))
                {
                    (bool success, string? res) = await UserApiService.GetUserToken(user.Email, user.Password);

                    if (success && res != null)
                    {
                        UserRepos.UpdateToken(user.Id, res);
                        userResponse.Token = res;
                    }
                    else
                    {
                        if (res != null)
                            userResponse.Error = ErrorType.WrongEmailOrPassword;
                    }
                    return userResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get user and add in sqlite
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string email, string password)
        {
            try
            {
                BookshelfModels.User.User user = await UserApiService.GetUser(email, password);

                if (user.Error is null)
                {
                    UserRepos.InsertUser(user);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { throw; }
        }

        public static void CleanUserDatabase() => UserRepos.CleanUserDatabase();

        public async Task<BookshelfModels.User.User> SignUp(string name, string email, string password) => await UserApiService.SignUp(name, email, password);
    }
}
