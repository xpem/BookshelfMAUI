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
        /// get user in the db local
        /// </summary>
        /// <returns></returns>
        public BookshelfModels.User.User? GetUserLocal() => UserRepos.GetUser();


        public async Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User? user)
        {
            (BookshelfModels.User.User? userResponse, bool Success) = await userAuthServices.RefreshUserToken(user);

            if (Success)
            {
                UserRepos.UpdateToken(user.Id, user.Token);
            }

            return userResponse;
        }

        /// <summary>
        /// Add a user in fb
        /// </summary>
        /// <returns></returns>
        public async Task<BookshelfModels.User.User?> InsertUser(string email, string password)
        {

            //BookshelfModels.User.User user = await userFbRepos.InsertUser(email, password);

            //if (user != null)
            //{
            //    if (user.Error != null)
            //    {
            //        return user;
            //    }
            //    else
            //    {
            //        userLocalRepos.AddUserLocal(user);
            //        return user;
            //    }
            //}
            //else
            return null;
        }
    }
}
