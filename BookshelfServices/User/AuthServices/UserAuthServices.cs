using BookshelfModels.User;
using Firebase.Auth;

namespace BookshelfServices.User.AuthServices
{
    public class UserAuthServices : IUserAuthServices
    {
        private readonly FirebaseAuthProvider authProvider = new(new FirebaseConfig(ApiKeys.ApiKey));

        public async Task<(BookshelfModels.User.User?, bool)> RefreshUserToken(BookshelfModels.User.User? user)
        {
            BookshelfModels.User.User? userNewToken = await SignInWithEmailAndPassword(user?.Email, user?.Password);

            if (user != null && userNewToken?.Token is not null)
            {
                if (user.Error != null)
                {
                    return (user, false);
                }
                else
                {
                    user.Token = userNewToken.Token;
                    return (user, true);
                }
            }
            else
                return (null, false);
        }

        public async Task<BookshelfModels.User.User?> SignInWithEmailAndPassword(string? email, string? password)
        {
            try
            {
                var fbTokens = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                return new BookshelfModels.User.User() { Token = fbTokens.FirebaseToken, Email = fbTokens.User.Email, Password = password, };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("WrongPassword"))
                {
                    return new BookshelfModels.User.User() { Error = ErrorType.WrongPassword, };
                }
                else if (ex.Message.Contains("UnknownEmailAddress"))
                {
                    return new BookshelfModels.User.User() { Error = ErrorType.UnknownEmailAddress, };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Add a user in fb
        /// </summary>
        public async Task<BookshelfModels.User.User?> CreateUser(string email, string password)
        {
            try
            {

                var fbTokens = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

                return new BookshelfModels.User.User() { Token = fbTokens.FirebaseToken, Email = fbTokens.User.Email, Password = password, };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    return new BookshelfModels.User.User() { Error = ErrorType.EMAIL_EXISTS, };
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task SendPasswordResetEmail(string email)
        {
            try
            {
                await authProvider.SendPasswordResetEmailAsync(email);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
