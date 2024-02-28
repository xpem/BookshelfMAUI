using ApiDAL.Interfaces;
using DbContextDAL;
using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Books;
using Models.Responses;
using Moq;

namespace BLL.Books.Tests
{
    [TestClass()]
    public class BooksBLLTests
    {
        [TestMethod()]
        public void GetBookshelfTotalsTest()
        {
            Mock<DbSet<Book>> mockSetBook = new();
            Mock<BookshelfDbContext> mockContext = new();

            List<TotalBooksGroupedByStatus> totalBooksGroupedByStatuses =
            [
                new TotalBooksGroupedByStatus
                {
                    Count = 1,
                    Status = Status.IllRead
                },
                 new TotalBooksGroupedByStatus
                {
                    Count = 3,
                    Status = Status.Read
                },
                  new TotalBooksGroupedByStatus
                {
                    Count = 2,
                    Status = Status.Reading
                },
                   new TotalBooksGroupedByStatus
                {
                    Count = 0,
                    Status = Status.Interrupted
                },
            ];

            Mock<IBookApiBLL> bookApiBLL = new();

            IBookDAL bookDAL = new BookDAL(mockContext.Object);

            Mock<IBookDAL> mockBookDAL = new();

            mockBookDAL.Setup(x => x.GetTotalBooksGroupedByStatusAsync(1)).ReturnsAsync(totalBooksGroupedByStatuses);

            Mock<IUserDAL> mockUserDAL = new();

            BooksBLL booksBLL = new(bookApiBLL.Object, mockBookDAL.Object);

            Totals? result = booksBLL.GetBookshelfTotalsAsync(1).Result;

            if (result is not null && result.IllRead == 1 && result.Reading == 2 && result.Read == 3 && result.Interrupted == 0)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }

        [TestMethod()]
        public void UpdateBook_FaiL_Validation_Unique_Title_Test()
        {
            Mock<BookshelfDbContext> mockContext = new();

            Mock<DbSet<Book>> mockSetBook = new();

            Book book =
                new()
                {
                    Title = "Teste de Título 6",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 1,
                    LocalTempId = "Temp1",
                    LocalId = 1,
                };

            Mock<DbSet<Models.User>> mockSetUser = new();

            IQueryable<Models.User> mockUsers = new List<Models.User>() {
                new()
                {
                    Id = 1,
                    Email="emanuel@teste.com.br",LastUpdate=DateTime.Now.AddHours(-2),
                    Name="Emanuel",
                }
            }.AsQueryable();

            mockSetUser.As<IQueryable<Models.User>>().Setup(m => m.Provider).Returns(mockUsers.Provider);
            mockSetUser.As<IQueryable<Models.User>>().Setup(m => m.Expression).Returns(mockUsers.Expression);
            mockSetUser.As<IQueryable<Models.User>>().Setup(m => m.ElementType).Returns(mockUsers.ElementType);
            mockSetUser.As<IQueryable<Models.User>>().Setup(m => m.GetEnumerator()).Returns(() => mockUsers.GetEnumerator());

            mockContext.Setup(m => m.User).Returns(mockSetUser.Object);

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            Mock<IBookApiBLL> bookApiBLL = new();
            Mock<IUserApiDAL> userAPIDAL = new();
            Mock<IUserDAL> mockUserDAL = new();

            Mock<IBookDAL> mockBookDAL = new();

            Book UptBook = new()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                Id = 2,
                LocalTempId = "Temp1",
                LocalId = 2
            };

            mockBookDAL.Setup(x => x.GetBookByTitleAsync(1, "Teste de Título 6")).ReturnsAsync(book);
            mockBookDAL.Setup(x => x.ExecuteUpdateBookAsync(It.IsAny<Book>())).ReturnsAsync(1);

            BLLResponse bLLResponse = new() { Success = true };

            bookApiBLL.Setup(x => x.UpdateBookAsync(It.IsAny<Book>())).ReturnsAsync(bLLResponse);

            BooksBLL booksBLL = new(bookApiBLL.Object, mockBookDAL.Object);

            Models.Responses.BLLResponse? result = booksBLL.UpdateBookAsync(1, UptBook).Result;

            if (result is not null && result.Success == false && result.Content is not null && ((string)result.Content) == "Livro com este título já cadastrado.")
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }
    }
}