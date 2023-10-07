﻿using ApiDAL.Interfaces;
using BLL.User;
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

            IQueryable<Book> mockBooks = new List<Book>()
            {
                new Book()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = null,
                LocalTempId = "Temp1"
            },
                new Book()
            {
                Title = "Teste de Título 2",
                Authors = "Emanuel Teste",
                Status = Status.Reading,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = null,
                LocalTempId = "Temp2"
            },
                     new Book() {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.Reading,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 3
                },
                new Book() {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.Read,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 4
                },
                   new Book() {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.Read,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                       Id = 5
                   },
                    new Book() {
                    Title = "Teste de Título 6",
                    Authors = "Emanuel Teste",
                    Status = Status.Read,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-6),
                    UserId = 1,
                       Id = 6
                   },
            }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

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

            mockContext.Setup(m => m.User).Returns(mockSetUser.Object);

            Mock<IBookApiBLL> bookApiBLL = new();

            Mock<IUserApiDAL> userAPIDAL = new();

            IBookDAL bookDAL = new BookDAL(mockContext.Object);

            IUserBLL userBLL = new UserBLL(userAPIDAL.Object, mockContext.Object);
            IBooksBLL booksBLL = new BooksBLL(bookApiBLL.Object, userBLL, bookDAL);

            Totals? result = booksBLL.GetBookshelfTotals();

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
                new Book()
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

            mockContext.Setup(m => m.User).Returns(mockSetUser.Object);

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            Mock<IBookApiBLL> bookApiBLL = new();
            Mock<IUserApiDAL> userAPIDAL = new();
            IUserBLL userBLL = new UserBLL(userAPIDAL.Object, mockContext.Object);

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

            bookApiBLL.Setup(x => x.UpdateBook(It.IsAny<Book>())).ReturnsAsync(bLLResponse);

            IBooksBLL booksBLL = new BooksBLL(bookApiBLL.Object, userBLL, mockBookDAL.Object);

            Models.Responses.BLLResponse? result = booksBLL.UpdateBook(UptBook).Result;

            if (result is not null && result.Success == false && result.Content is not null && ((string)result.Content) == "Livro com este título já cadastrado.")
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }
    }
}