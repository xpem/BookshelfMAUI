using ApiDAL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Responses;
using Moq;
using Repositories.Interfaces;
using Services;
using Services.User;

namespace BLLTests.User
{
    [TestClass()]
    public class UserServiceTests
    {

        readonly Mock<IUserApiDAL> userApiDAL = new();
        readonly Mock<IUserRepo> userRepo = new();
        readonly Mock<IBuildDbBLL> buildDbBLL = new();

        [TestMethod()]
        public async Task SignIn_Success_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "121212";

            (bool, string?) respToken = (true, "test");

            ApiResponse apiResponse = new ApiResponse() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
            Models.DTOs.User? actualUser = null;
            BLLResponse respSuccess = new() { Success = true, Content = 1 };


            userApiDAL.Setup(x => x.GetUserTokenAsync(email, password)).ReturnsAsync(respToken);
            userApiDAL.Setup(x => x.GetUserAsync(respToken.Item2)).ReturnsAsync(apiResponse);
            userRepo.Setup(x => x.GetUserLocalAsync()).ReturnsAsync(actualUser);
            userRepo.Setup(x => x.CreateAsync(It.IsAny<Models.DTOs.User?>())).ReturnsAsync(1);

            UserService userService = new(userApiDAL.Object, userRepo.Object, buildDbBLL.Object);

            var respSignIn = await userService.SignIn(email, password);

            if (respSignIn != null && respSignIn.Success)
            {
                Assert.AreEqual(respSignIn.Content, 1);
            }
            else
                Assert.Fail();
        }

        [TestMethod()]
        public async Task SignIn_WrongCredentials_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "111212";

            (bool, string?) respToken = (false, "User/Password incorrect");

            BLLResponse respSuccess = new() { Success = false, Error = ErrorTypes.WrongEmailOrPassword };

            userApiDAL.Setup(x => x.GetUserTokenAsync(email, password)).ReturnsAsync(respToken);

            UserService userService = new(userApiDAL.Object, userRepo.Object, buildDbBLL.Object);

            var respSignIn = await userService.SignIn(email, password);

            if (respSignIn != null && !respSignIn.Success)
            {
                Assert.AreEqual(respSignIn.Error, ErrorTypes.WrongEmailOrPassword);
            }
            else
                Assert.Fail();
        }

        [TestMethod()]
        public async Task ReSignIn_Sucess_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "121212";

            (bool, string?) respToken = (true, "test");

            ApiResponse apiResponse = new ApiResponse() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
            Models.DTOs.User? actualUser = new Models.DTOs.User() { Id = 1, Email = "emanuel.teste@email.com", LastUpdate = DateTime.Now, Name = "Emanuel Martins", Password = "121212", Token = "test" };
            BLLResponse respSuccess = new() { Success = true, Content = 1 };


            userApiDAL.Setup(x => x.GetUserTokenAsync(email, password)).ReturnsAsync(respToken);
            userApiDAL.Setup(x => x.GetUserAsync(respToken.Item2)).ReturnsAsync(apiResponse);
            userRepo.Setup(x => x.GetUserLocalAsync()).ReturnsAsync(actualUser);
            userRepo.Setup(x => x.UpdateAsync(It.IsAny<Models.DTOs.User?>()));

            UserService userService = new(userApiDAL.Object, userRepo.Object, buildDbBLL.Object);

            var respSignIn = await userService.SignIn(email, password);

            if (respSignIn != null && respSignIn.Success)
            {
                Assert.AreEqual(respSignIn.Content, 1);
                userRepo.Verify(x => x.UpdateAsync(It.IsAny<Models.DTOs.User?>()), Times.Exactly(1));
            }
            else
                Assert.Fail();
        }

        [TestMethod()]
        public async Task ReSignIn_AnotherUser_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "121212";

            (bool, string?) respToken = (true, "test");

            ApiResponse apiResponse = new ApiResponse() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
            Models.DTOs.User? actualUser = new Models.DTOs.User() { Id = 2, Email = "emanuel.teste@email.com", LastUpdate = DateTime.Now, Name = "Emanuel Martins", Password = "121212", Token = "test" };

            BLLResponse respSuccess = new() { Success = true, Content = 1 };


            userApiDAL.Setup(x => x.GetUserTokenAsync(email, password)).ReturnsAsync(respToken);
            userApiDAL.Setup(x => x.GetUserAsync(respToken.Item2)).ReturnsAsync(apiResponse);
            userRepo.Setup(x => x.GetUserLocalAsync()).ReturnsAsync(actualUser);
            userRepo.Setup(x => x.CreateAsync(It.IsAny<Models.DTOs.User?>())).ReturnsAsync(1);

            UserService userService = new(userApiDAL.Object, userRepo.Object, buildDbBLL.Object);

            var respSignIn = await userService.SignIn(email, password);

            if (respSignIn != null && respSignIn.Success)
            {
                Assert.AreEqual(respSignIn.Content, 1);
                buildDbBLL.Verify(x => x.CleanLocalDatabase(), Times.Exactly(1));
                userRepo.Verify(x => x.CreateAsync(It.IsAny<Models.DTOs.User?>()), Times.Exactly(1));
            }
            else
                Assert.Fail();
        }
    }
}