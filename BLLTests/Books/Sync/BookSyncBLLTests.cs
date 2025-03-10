﻿using ApiDAL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Responses;
using Moq;
using Xunit;
using Models.DTOs;
using Models.DTOs.OperationQueue;
using Services.Books.Interfaces;
using Services.Books.Sync;
using Repos;
using Repos.Interfaces;

namespace BLLTests.Books.Sync
{
    [TestClass()]
    public class BookSyncBLLTests
    {
        readonly Mock<IOperationQueueRepo> mockOperationQueueDAL = new();
        readonly Mock<IBookApiService> mockBookApiBLL = new();
        readonly Mock<IBookRepo> mockBookDAL = new();

        [TestMethod()]
        public async Task ApiToLocalSync_CreateLocalBooksTest()
        {
            DateTime lastUpdate = DateTime.MinValue;

            List<Book> apiResultBooks =
            [
                new Book()
                {
                    Title = "Teste de Título",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 1,
                    LocalId = 1,
                },
                new Book()
                {
                    Title = "Teste de Título 2",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-3),
                    UserId = 1,
                    Id = 2,
                    LocalId = 2,
                },
                new Book()
                {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(-2),
                    UserId = 1,
                    Id = 3,
                    LocalId = 3,
                },
                new Book()
                {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddHours(-1),
                    UserId = 1,
                    Id = 4,
                    LocalId = 4,
                },
                new Book()
                {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = 1,
                    Id = 5,
                    LocalId = 5,
                },
            ];

            IQueryable<Book> mockBooks = new List<Book>()
            {
                new() {
                    Title = "Teste de Título 6",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 6, LocalId = 6,
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

            Mock<IBookApiService> bookApiBLL = new();

            BLLResponse bLLResponse = new() { Success = true, Content = apiResultBooks };
            Book? bookLastUpdate = null;

            bookApiBLL.Setup(x => x.GetByLastUpdateAsync(lastUpdate, 1)).ReturnsAsync(() => bLLResponse);
            mockBookDAL.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(bookLastUpdate);
            mockBookDAL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(1);

            BookSyncService bookSyncBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockOperationQueueDAL.Object);

            await bookSyncBLL.ApiToLocalSync(1, lastUpdate);

            mockBookDAL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Exactly(5));
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(0));

            //if (added == 5 && updated == 0)
            //    Assert.IsTrue(true);
            //else
            //Assert.Fail();
        }

        [TestMethod()]
        public async Task ApiToLocalSync_Create_and_UpdateLocalBooksTest()
        {
            Mock<Microsoft.EntityFrameworkCore.DbSet<Book>> mockSetBook = new();

            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            List<Book> apiResultBooks =
            [
                new Book()
                {
                    Title = "Teste de Título",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 1,
                    LocalId = 1
                },
                new Book()
                {
                    Title = "Teste de Título 2",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    UserId = 1,
                    Id = 2,
                    LocalId = 2
                },
                new Book()
                {
                    Title = "Teste de Título 3",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 3,
                    LocalId = 3
                },
                new Book()
                {
                    Title = "Teste de Título 4",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 4,
                    LocalId = 4
                },
                new Book()
                {
                    Title = "Teste de Título 5",
                    Authors = "Emanuel Teste",
                    Status = Status.IllRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now.AddDays(-5),
                    UserId = 1,
                    Id = 5,
                    LocalId = 5
                },
            ];

            Book Book1 = new()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-5),
                UserId = 1,
                Id = 1,
                LocalId = 1
            };

            Book Book1ByIdReturn = new()
            {
                Title = "Teste de Título 6",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-6),
                UserId = 1,
                Id = 1,
                LocalId = 1
            };
            Book Book2 = new()
            {
                Title = "Teste de Título Alterado",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-5),
                UserId = 1,
                Id = 2,
                LocalId = 2
            };

            Book Book2ByIdReturn = new()
            {
                Title = "Teste de Título Alterado",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-7),
                UserId = 1,
                Id = 2,
                LocalId = 2
            };

            IQueryable<Book> mockBooks = new List<Book>() { Book1, Book2 }.AsQueryable();

            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(mockBooks.Provider);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(mockBooks.Expression);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(mockBooks.ElementType);
            mockSetBook.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => mockBooks.GetEnumerator());

            Mock<BookshelfDbContext> mockContext = new();

            mockContext.Setup(m => m.Book).Returns(mockSetBook.Object);

            Mock<IBookApiService> bookApiBLL = new();

            Mock<IBookRepo> mockBookDAL = new();

            //mockBookDAL.Setup(x => x.ExecuteUpdateBookAsync(Book1)).ReturnsAsync(1);

            mockBookDAL.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Book1ByIdReturn);
            mockBookDAL.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(Book2ByIdReturn);
            mockBookDAL.Setup(x => x.UpdateAsync(Book1)).ReturnsAsync(1);

            BLLResponse bLLResponse = new() { Success = true, Content = apiResultBooks };

            bookApiBLL.Setup(x => x.GetByLastUpdateAsync(lastUpdate, 1)).ReturnsAsync(() => bLLResponse);

            Mock<IHttpClientFunctions> mockHttpClientFunctions = new();
            Mock<IOperationQueueRepo> mockOperationQueueDAL = new();

            BookSyncService bookSyncBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockOperationQueueDAL.Object);

            await bookSyncBLL.ApiToLocalSync(1, lastUpdate);

            mockBookDAL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Exactly(3));
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(2));

            //if (added == 3 && updated == 2)
            //    Assert.IsTrue(true);
            //else
            //    Assert.Fail();
        }

        [Fact(DisplayName = "Executa as ops utilizando a fila de operçãoes adicionando dois livros via api")]
        [TestMethod()]
        public async Task LocalToApiSync_ApiAddBook_Test()
        {
            DateTime lastUpdate = DateTime.Now.AddDays(-3);
            ApiOperation insertBook1Op = new()
            {
                Id = 1,
                Content = "{\"LocalId\":2,\"Id\":null,\"UserId\":0,\"LTempId\":\"01d00c2f-6137-4afe-9f55-73eb533e1963\",\"Title\":\"livro um\",\"SubTitle\":\"subt\\u00EDtulo um\",\"Authors\":\"autor um\",\"Volume\":12,\"Pages\":123,\"Year\":2023,\"Status\":1,\"Genre\":\"\",\"Isbn\":\"\",\"Cover\":\"\",\"GoogleId\":null,\"Score\":0,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-01T18:15:48.3594026-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Insert,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ObjectId = "2",
                Status = ApiOperationStatus.Pending
            };

            ApiOperation insertBook2Op = new()
            {
                Id = 2,
                Content = "{\"LocalId\":3,\"Id\":null,\"UserId\":0,\"Title\":\"livro dois\",\"SubTitle\":\"subt\\u00EDtulo dois\",\"Authors\":\"autor dois\",\"Volume\":12,\"Pages\":123,\"Year\":2023,\"Status\":1,\"Genre\":\"\",\"Isbn\":\"\",\"Cover\":\"\",\"GoogleId\":null,\"Score\":0,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-01T18:15:48.3594026-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Insert,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ObjectId = "3",
                Status = ApiOperationStatus.Pending
            };

            List<ApiOperation> PendingOperations =
            [
               insertBook1Op,
                insertBook2Op
            ];

            ApiResponse insertBook1response = new() { Success = true, Content = "100" };
            ApiResponse insertBook2response = new() { Success = true, Content = "120" };

            DateTime datetimeNow = DateTime.Now;

            Book insertBook1Local = new()
            {
                Title = "livro um",
                SubTitle = "subtítulo um",
                Authors = "autor um",
                Volume = 12,
                Pages = 123,
                Year = 2023,
                Status = Status.IllRead,
                Genre = "",
                CreatedAt = DateTime.MinValue,
                UpdatedAt = datetimeNow,
                Inactive = false,
                Score = 0,
                Isbn = "",
                LocalId = 2
            };

            Book insertBook2Local = new()
            {
                Title = "livro dois",
                SubTitle = "subtítulo dois",
                Authors = "autor dois",
                Volume = 12,
                Pages = 123,
                Year = 2023,
                Status = Status.IllRead,
                Genre = "",
                CreatedAt = DateTime.MinValue,
                UpdatedAt = datetimeNow,
                Inactive = false,
                Score = 0,
                Isbn = "",
                LocalId = 3
            };
            BLLResponse bLL1Response = new() { Success = true, Content = 10 };
            BLLResponse bLL2Response = new() { Success = true, Content = 20 };

            mockOperationQueueDAL.Setup(x => x.GetPendingOperationsByStatusAsync(ApiOperationStatus.Pending)).ReturnsAsync(PendingOperations);
            mockBookApiBLL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(bLL1Response);
            mockBookDAL.Setup(x => x.UpdateAsync(It.IsAny<Book>()));
            mockOperationQueueDAL.Setup(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook1Op.Id));

            BookSyncService bookSyncBLL = new(mockBookApiBLL.Object, mockBookDAL.Object, mockOperationQueueDAL.Object);

            await bookSyncBLL.LocalToApiSync(1, lastUpdate);

            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(2));
            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook1Op.Id), Times.Once());
            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook2Op.Id), Times.Once());

            //if (added == 2)
            //    Assert.IsTrue(true);
            //else
            //    Assert.Fail();
        }

        [Fact(DisplayName = "Adiciona um livro e o atualiza via as operações.")]
        [TestMethod()]
        public async Task LocalToApiSync_ApiAdd_And_Update_Same_Book_Test()
        {

            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            ApiOperation insertBookOp = new()
            {
                Id = 1,
                Content = "{\"LocalId\":36,\"Id\":null,\"UserId\":6,\"LocalTempId\":\"35d624bd-f7df-49c4-979c-784dc59a4f90\",\"Title\":\"teste livro add\",\"SubTitle\":\"subs add\",\"Authors\":\"teste add\",\"Volume\":21,\"Pages\":32,\"Year\":2013," +
                "\"Status\":1,\"Genre\":\"\",\"Isbn\":\"\",\"Cover\":\"\",\"GoogleId\":null,\"Score\":0,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-06T07:50:11.6830278-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Insert,
                ObjectId = "36",
                CreatedAt = new DateTime(2024, 03, 06, 07, 50, 14),
                UpdatedAt = new DateTime(2024, 03, 06, 07, 50, 14),
                Status = ApiOperationStatus.Pending,
            };

            ApiOperation updateBookOp = new()
            {
                Id = 2,
                Content = "{\"LocalId\":36,\"Id\":null,\"UserId\":6,\"LocalTempId\":null,\"Title\":\"teste livro add\",\"SubTitle\":\"subs add\",\"Authors\":\"teste add\",\"Volume\":21,\"Pages\":32,\"Year\":2013,\"Status\":3,\"Genre\":\"\",\"Isbn\":\"\"," +
                "\"Cover\":\"\",\"GoogleId\":null,\"Score\":3,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-06T07:50:38.9655821-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Update,
                ObjectId = "36",
                CreatedAt = new DateTime(2024, 03, 06, 07, 50, 38),
                UpdatedAt = new DateTime(2024, 03, 06, 07, 50, 38),
                Status = ApiOperationStatus.Pending,
            };

            Book insertBook1Local = new()
            {
                Title = "teste livro add",
                SubTitle = "subs add",
                Authors = "teste add",
                Volume = 21,
                Pages = 32,
                Year = 2013,
                Status = Status.IllRead,
                Genre = "",
                CreatedAt = DateTime.MinValue,
                UpdatedAt = lastUpdate,
                Inactive = false,
                Score = 0,
                Isbn = "",
                LocalId = 3
            };

            Book updatedBook1Local = new()
            {
                Title = "teste livro add",
                SubTitle = "subs add",
                Authors = "teste add",
                Volume = 21,
                Pages = 32,
                Year = 2013,
                Status = Status.IllRead,
                Genre = "",
                CreatedAt = DateTime.MinValue,
                UpdatedAt = lastUpdate,
                Inactive = false,
                Score = 0,
                Isbn = "",
                LocalId = 3,
                Id = 100,
            };

            BLLResponse insertBook1Response = new() { Success = true, Content = "100" };
            BLLResponse uptBook1Response = new() { Success = true };

            var pendingOperations = new List<ApiOperation>() { insertBookOp, updateBookOp };


            mockOperationQueueDAL.Setup(x => x.GetPendingOperationsByStatusAsync(ApiOperationStatus.Pending))
                .ReturnsAsync(pendingOperations);
            mockBookApiBLL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(insertBook1Response);
            mockBookApiBLL.Setup(c => c.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(uptBook1Response);
            mockBookDAL.Setup(x => x.GetBookByLocalIdAsync(6, 36)).ReturnsAsync(updatedBook1Local);

            BookSyncService bookSyncBLL = new(mockBookApiBLL.Object, mockBookDAL.Object, mockOperationQueueDAL.Object);
            await bookSyncBLL.LocalToApiSync(1, lastUpdate);

            mockBookApiBLL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Once());
            mockBookApiBLL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Once());
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(1));
            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, 1), Times.Once);
            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, 2), Times.Once);
        }

        [Fact(DisplayName = "Adiciona um livro e atualiza outro")]
        [TestMethod()]
        public async Task LocalToApiSync_ApiAdd_And_Update_Books_Test()
        {
            DateTime lastUpdate = DateTime.Now.AddDays(-3);
            ApiOperation insertBook1Op = new()
            {
                Id = 1,
                Content = "{\"LocalId\":2,\"Id\":null,\"UserId\":0,\"LTempId\":\"01d00c2f-6137-4afe-9f55-73eb533e1963\",\"Title\":\"livro um\",\"SubTitle\":\"subt\\u00EDtulo um\",\"Authors\":\"autor um\",\"Volume\":12,\"Pages\":123,\"Year\":2023,\"Status\":1,\"Genre\":\"\",\"Isbn\":\"\",\"Cover\":\"\",\"GoogleId\":null,\"Score\":0,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-01T18:15:48.3594026-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Insert,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ObjectId = "2",
                Status = ApiOperationStatus.Pending
            };

            ApiOperation uptBook1Op = new()
            {
                Id = 2,
                Content = "{\"LocalId\":3,\"Id\":null,\"UserId\":0,\"Title\":\"livro dois alterado\",\"SubTitle\":\"subt\\u00EDtulo dois\",\"Authors\":\"autor dois\",\"Volume\":12,\"Pages\":123,\"Year\":2023,\"Status\":1,\"Genre\":\"\",\"Isbn\":\"\",\"Cover\":\"\",\"GoogleId\":null,\"Score\":0,\"Comment\":null,\"CreatedAt\":\"0001-01-01T00:00:00\",\"UpdatedAt\":\"2024-03-01T18:15:48.3594026-03:00\",\"Inactive\":false}",
                ObjectType = ObjectType.Book,
                ExecutionType = ExecutionType.Update,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ObjectId = "3",
                Status = ApiOperationStatus.Pending
            };

            List<ApiOperation> PendingOperations =
            [
               insertBook1Op,
                uptBook1Op
            ];

            BLLResponse insertBook1Response = new() { Success = true, Content = "100" };
            BLLResponse uptBook2Response = new() { Success = true };

            DateTime datetimeNow = DateTime.Now;

            Book insertBook1Local = new()
            {
                Title = "livro um",
                SubTitle = "subtítulo um",
                Authors = "autor um",
                Volume = 12,
                Pages = 123,
                Year = 2023,
                Status = Status.IllRead,
                Genre = "",
                CreatedAt = DateTime.MinValue,
                UpdatedAt = datetimeNow,
                Inactive = false,
                Score = 0,
                Isbn = "",
                LocalId = 3,
                UserId = 0
            };

            mockOperationQueueDAL.Setup(x => x.GetPendingOperationsByStatusAsync(ApiOperationStatus.Pending)).ReturnsAsync(PendingOperations);
            mockBookApiBLL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(insertBook1Response);
            mockBookApiBLL.Setup(c => c.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(uptBook2Response);
            mockBookDAL.Setup(x => x.GetBookByLocalIdAsync(0, 3)).ReturnsAsync(insertBook1Local);
            mockOperationQueueDAL.Setup(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook1Op.Id));

            mockBookDAL.Setup(x => x.UpdateAsync(It.IsAny<Book>()));

            BookSyncService bookSyncBLL = new(mockBookApiBLL.Object, mockBookDAL.Object, mockOperationQueueDAL.Object);

            await bookSyncBLL.LocalToApiSync(1, lastUpdate);

            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook1Op.Id), Times.Once());
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Once);
            mockOperationQueueDAL.Verify(x => x.UpdateOperationStatusAsync(ApiOperationStatus.Success, insertBook1Op.Id), Times.Once());
            mockBookApiBLL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Once);
            mockBookApiBLL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Once);

        }
    }
}