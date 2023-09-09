using ApiDAL.Interfaces;
using BLL.Books.Historic.Interfaces;
using BLL.User;
using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Books.Historic;
using Moq;

namespace BLL.Books.Historic.Tests
{
    [TestClass()]
    public class BookHistoricBLLTests
    {
        [TestMethod()]
        public void GetBookHistoricByBookIdTest()
        {
            Mock<BookshelfDbContext> mockContext = new();

            Mock<DbSet<Models.User>> mockSetUser = new();

            IQueryable<Models.User> mockUsers = new List<Models.User>() {
                new Models.User()
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


            IQueryable<BookHistoric> mockBookHistorics = new List<BookHistoric>() {
                new BookHistoric()
             {
                 Id = 7,
                 BookId  = 1,
                 CreatedAt= DateTime.Now.AddDays(-2),
                    Uid=1,
                    TypeId=2,
                    TypeName="Update",
                 BookHistoricItems =  new List<BookHistoricItem>() {
                     new BookHistoricItem()
                     {

                         BookHistoricId = 7,
                         BookFieldId = 5,
                         BookFieldName = "Volume",
                         CreatedAt = DateTime.Now.AddDays(-2),
                         UpdatedFrom = "4",
                         UpdatedTo = "5",
                         Id = 11,
                     },
                     new BookHistoricItem()
                     {
                         BookHistoricId = 7,
                         BookFieldId = 6,
                         BookFieldName = "Páginas",
                         CreatedAt = DateTime.Now.AddDays(-2),
                         UpdatedFrom = "265",
                         UpdatedTo = "275",
                         Id = 12,
                     },
                     new BookHistoricItem()
                     {
                         BookHistoricId = 7,
                         BookFieldId = 7,BookFieldName = "Ano",
                         CreatedAt = DateTime.Now.AddDays(-2),
                         UpdatedFrom = "2038",
                         UpdatedTo = "2048",
                         Id = 13,
                     }
                 },
             },
                new BookHistoric()
                {
                    Id = 6, TypeName="Update",
                    CreatedAt= DateTime.Now.AddDays(-1),
                    TypeId = 2,
                    BookId = 1,
                    Uid= 1,
                    BookHistoricItems =  new List<BookHistoricItem>() {
                        new BookHistoricItem()
                        {
                            BookHistoricId = 6,
                            BookFieldId = 6,
                            BookFieldName = "Páginas",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            UpdatedFrom = "3",
                            UpdatedTo = "4",
                            Id = 14,
                        },
                        new BookHistoricItem()
                        {
                            BookHistoricId = 6,
                            BookFieldId = 6,
                            BookFieldName ="Autores",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            UpdatedFrom = "teste 3 alteracao 2",
                            UpdatedTo = "teste 3 alteracao autor 3",
                            Id = 15,
                        }
                    }
                },
            }.AsQueryable();

            Mock<DbSet<BookHistoric>> mockSetBookHistorics = new();

            mockSetBookHistorics.As<IQueryable<BookHistoric>>().Setup(m => m.Provider).Returns(mockBookHistorics.Provider);
            mockSetBookHistorics.As<IQueryable<BookHistoric>>().Setup(m => m.Expression).Returns(mockBookHistorics.Expression);
            mockSetBookHistorics.As<IQueryable<BookHistoric>>().Setup(m => m.ElementType).Returns(mockBookHistorics.ElementType);
            mockSetBookHistorics.As<IQueryable<BookHistoric>>().Setup(m => m.GetEnumerator()).Returns(() => mockBookHistorics.GetEnumerator());

            mockContext.Setup(m => m.BookHistoric).Returns(mockSetBookHistorics.Object);
            mockContext.Setup(m => m.User).Returns(mockSetUser.Object);

            Mock<IUserApiDAL> userAPIDAL = new();

            IUserBLL userBLL = new UserBLL(userAPIDAL.Object, mockContext.Object);

            IBookHistoricBLL bookHistoricBLL = new BookHistoricBLL(userBLL, mockContext.Object);

            BookHistoricList result = bookHistoricBLL.GetBookHistoricByBookId(1, 1);

            if (result is not null && result.Total == 2)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }
    }
}