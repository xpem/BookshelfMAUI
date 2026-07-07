using ApiDAL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Responses;
using Moq;
using Repos.Interfaces;
using Services;
using Services.User;
using System.Text.Json;

namespace BLLTests.User
{
    [TestClass()]
    public class UserServiceTests
    {

        readonly Mock<IUserApiDAL> userApiDAL = new();
        readonly Mock<IUserRepo> userRepo = new();
        readonly Mock<IBuildDbService> buildDbBLL = new();

        [TestMethod()]
        public async Task SignIn_Success_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "121212";

            string tokenContent = JsonSerializer.Serialize(new { token = "test", refreshToken = "refresh_test" });
            ApiResponse tokenResponse = new() { Success = true, Content = tokenContent };

            ApiResponse apiResponse = new() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
            Models.DTOs.User? actualUser = null;


            userApiDAL.Setup(x => x.GetTokenAsync(email, password)).ReturnsAsync(tokenResponse);
            userApiDAL.Setup(x => x.GetUserAsync("test")).ReturnsAsync(apiResponse);
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

            ApiResponse tokenResponse = new() { Success = false, Content = "User/Password incorrect", Error = ErrorTypes.WrongEmailOrPassword };

            userApiDAL.Setup(x => x.GetTokenAsync(email, password)).ReturnsAsync(tokenResponse);

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

            string tokenContent = JsonSerializer.Serialize(new { token = "test", refreshToken = "refresh_test" });
            ApiResponse tokenResponse = new() { Success = true, Content = tokenContent };

            ApiResponse apiResponse = new() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
#pragma warning disable CS0612
            Models.DTOs.User? actualUser = new() { Id = 1, Email = "emanuel.teste@email.com", LastUpdate = DateTime.Now, Name = "Emanuel Martins", Token = "test", RefreshToken = "old_refresh" };
#pragma warning restore CS0612


            userApiDAL.Setup(x => x.GetTokenAsync(email, password)).ReturnsAsync(tokenResponse);
            userApiDAL.Setup(x => x.GetUserAsync("test")).ReturnsAsync(apiResponse);
            userRepo.Setup(x => x.GetUserLocalAsync()).ReturnsAsync(actualUser);
            userRepo.Setup(x => x.UpdateAsync(It.IsAny<Models.DTOs.User>()));

            UserService userService = new(userApiDAL.Object, userRepo.Object, buildDbBLL.Object);

            var respSignIn = await userService.SignIn(email, password);

            if (respSignIn != null && respSignIn.Success)
            {
                Assert.AreEqual(respSignIn.Content, 1);
                userRepo.Verify(x => x.UpdateAsync(It.IsAny<Models.DTOs.User>()), Times.Exactly(1));
            }
            else
                Assert.Fail();
        }

        [TestMethod()]
        public async Task ReSignIn_AnotherUser_Test()
        {
            string email = "emanuel_teste@email.com";
            string password = "121212";

            string tokenContent = JsonSerializer.Serialize(new { token = "test", refreshToken = "refresh_test" });
            ApiResponse tokenResponse = new() { Success = true, Content = tokenContent };

            ApiResponse apiResponse = new() { Content = "{\"id\":1,\"name\":\"Emanuel Martins\",\"email\":\"emanuel.teste@email.com\",\"createdAt\":\"2023-08-12T12:43:22.24644\"}", Success = true };
#pragma warning disable CS0612
            Models.DTOs.User? actualUser = new() { Id = 2, Email = "emanuel.teste@email.com", LastUpdate = DateTime.Now, Name = "Emanuel Martins", Token = "test", RefreshToken = "old_refresh" };
#pragma warning restore CS0612

            userApiDAL.Setup(x => x.GetTokenAsync(email, password)).ReturnsAsync(tokenResponse);
            userApiDAL.Setup(x => x.GetUserAsync("test")).ReturnsAsync(apiResponse);
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
