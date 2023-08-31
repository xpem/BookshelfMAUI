﻿using BLL.Books.Sync;
using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Books;
using Models.Responses;
using Moq;
using System.Net.Sockets;

namespace BLL.Books.Sync.Tests
{
    [TestClass()]
    public class BookSyncBLLTests
    {
        [TestMethod()]
        public void ApiToLocalSync_CreateLocalBooksTest()
        {

            Mock<DbSet<Book>> mockSetBook = new();

            DateTime lastUpdate = DateTime.MinValue;

            List<Book> apiResultBooks = new()
            {
                new Book() {
                    Title = "Teste de Título",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 1
                },
                new Book() {
                    Title = "Teste de Título 2",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-3),
                    UserId = 1,
                    Id = 2
                },
                     new Book() {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(-2),
                    UserId = 1,
                    Id = 3
                },
                new Book() {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(-1),
                    UserId = 1,
                    Id = 4
                },
                   new Book() {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = 1,
                       Id = 5
                   },
            };

            IQueryable<Book> mockBooks = new List<Book>()
            {
                new Book() {
                    Title = "Teste de Título 6",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 6
                },
                //new Book() {
                //    Title = "Teste de Título 2",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddDays(-3),
                //    UserId = 1,
                //    Id = 2
                //},
                //     new Book() {
                //    Title = "Teste de Título 3",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddHours(-2),
                //    UserId = 1,
                //    Id = 3
                //},
                //new Book() {
                //    Title = "Teste de Título 4",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddHours(-1),
                //    UserId = 1,
                //    Id = 4
                //},
                //   new Book() {
                //    Title = "Teste de Título 5",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now,
                //    UserId = 1,
                //       Id = 5
                //   },
            }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            Mock<BookshelfDbContext> mockContext = new();

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            var bookApiBLL = new Mock<IBookApiBLL>();
            var bLLResponse = new BLLResponse() { Success = true, Content = apiResultBooks };

            bookApiBLL.Setup(x => x.GetBooksByLastUpdate(lastUpdate)).ReturnsAsync(() => bLLResponse);

            IBookSyncBLL bookSyncBLL = new BookSyncBLL(bookApiBLL.Object, mockContext.Object);

            (int added, int updated) = bookSyncBLL.ApiToLocalSync(1, lastUpdate).Result;

            if (added == 5 && updated == 0)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }

        [TestMethod()]
        public void ApiToLocalSync_Create_and_UpdateLocalBooksTest()
        {
            Mock<DbSet<Book>> mockSetBook = new();

            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            List<Book> apiResultBooks = new()
            {
                new Book() {
                    Title = "Teste de Título",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 1
                },
                new Book() {
                    Title = "Teste de Título 2",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 2
                },
                     new Book() {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 3
                },
                new Book() {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 4
                },
                   new Book() {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                       Id = 5
                   },
            };

            IQueryable<Book> mockBooks = new List<Book>()
            {
                new Book() {
                    Title = "Teste de Título 6",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 1
                },
                new Book() {
                    Title = "Teste de Título Alterado",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 2
                },
                //new Book() {
                //    Title = "Teste de Título 2",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddDays(-3),
                //    UserId = 1,
                //    Id = 2
                //},
                //     new Book() {
                //    Title = "Teste de Título 3",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddHours(-2),
                //    UserId = 1,
                //    Id = 3
                //},
                //new Book() {
                //    Title = "Teste de Título 4",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now.AddHours(-1),
                //    UserId = 1,
                //    Id = 4
                //},
                //   new Book() {
                //    Title = "Teste de Título 5",
                //    Authors = "Emanuel Teste",
                //    Status = Status.IllRead,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now,
                //    UserId = 1,
                //       Id = 5
                //   },
            }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            Mock<BookshelfDbContext> mockContext = new();

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            var bookApiBLL = new Mock<IBookApiBLL>();
            var bLLResponse = new BLLResponse() { Success = true, Content = apiResultBooks };

            bookApiBLL.Setup(x => x.GetBooksByLastUpdate(lastUpdate)).ReturnsAsync(() => bLLResponse);

            IBookSyncBLL bookSyncBLL = new BookSyncBLL(bookApiBLL.Object, mockContext.Object);

            (int added, int updated) = bookSyncBLL.ApiToLocalSync(1, lastUpdate).Result;

            if (added == 3 && updated == 2)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }

        [TestMethod()]
        public void LocalToApiSync_ApiAddBook_Test()
        {
            Mock<DbSet<Book>> mockSetBook = new();

            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            Book bookForAddInApi1 = new Book()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = null,
                LocalTempId = "Temp1"
            };

            Book bookForAddInApi2 = new Book()
            {
                Title = "Teste de Título 2",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = null,
                LocalTempId = "Temp2"
            };

            IQueryable<Book> mockBooks = new List<Book>()
            {
                bookForAddInApi1,
                bookForAddInApi2,
                     new Book() {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 3
                },
                new Book() {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 4
                },
                   new Book() {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                       Id = 5
                   },
            }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            Mock<BookshelfDbContext> mockContext = new();

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            var bookApiBLL = new Mock<IBookApiBLL>();

            var bLLResponse1 = new BLLResponse() { Success = true, Content = 1 };
            var bLLResponse2 = new BLLResponse() { Success = true, Content = 2 };

            bookApiBLL.Setup(x => x.AddBook(bookForAddInApi1)).ReturnsAsync(() => bLLResponse1);
            bookApiBLL.Setup(x => x.AddBook(bookForAddInApi2)).ReturnsAsync(() => bLLResponse2);

            IBookSyncBLL bookSyncBLL = new BookSyncBLL(bookApiBLL.Object, mockContext.Object);

            (int added, int updated) = bookSyncBLL.LocalToApiSync(1, lastUpdate).Result;

            if (added == 2 && updated == 0)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }

        [TestMethod()]
        public void LocalToApiSync_ApiAdd_And_Update_Book_Test()
        {

            Mock<DbSet<Book>> mockSetBook = new();

            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            Book bookForAddInApi1 = new Book()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = null,
                LocalTempId = "Temp1"
            };

            Book bookForUptInApi1 = new Book()
            {
                Title = "Teste de Título 2",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = 2,
            };

            Book bookForUptInApi2 = new Book()
            {
                Title = "Teste de Título Alterado",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-2),
                UserId = 1,
                Id = 3
            };

            IQueryable<Book> mockBooks = new List<Book>()
            {
                bookForAddInApi1,
                bookForUptInApi1,
                bookForUptInApi2,
                new Book() {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 4
                },
                   new Book() {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                       Id = 5
                   },
            }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            Mock<BookshelfDbContext> mockContext = new();

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            var bookApiBLL = new Mock<IBookApiBLL>();

            var bLLResponse1 = new BLLResponse() { Success = true, Content = 1 };
            var bLLResponse2 = new BLLResponse() { Success = true, Content = 2 };
            var bLLResponse3 = new BLLResponse() { Success = true, Content = 3 };

            bookApiBLL.Setup(x => x.AddBook(bookForAddInApi1)).ReturnsAsync(() => bLLResponse1);
            bookApiBLL.Setup(x => x.UpdateBook(bookForUptInApi1)).ReturnsAsync(() => bLLResponse2);
            bookApiBLL.Setup(x => x.UpdateBook(bookForUptInApi2)).ReturnsAsync(() => bLLResponse3);

            IBookSyncBLL bookSyncBLL = new BookSyncBLL(bookApiBLL.Object, mockContext.Object);

            (int added, int updated) = bookSyncBLL.LocalToApiSync(1, lastUpdate).Result;

            if (added == 1 && updated == 2)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }
    }
}