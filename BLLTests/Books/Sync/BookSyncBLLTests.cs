using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Responses;
using Moq;
using Xunit;
using Models.DTOs;
using Services.Books.Interfaces;
using Services.Books.Sync;
using Repos;
using Repos.Interfaces;

namespace BLLTests.Books.Sync
{
    [TestClass()]
    public class BookSyncBLLTests
    {
        readonly Mock<ISyncCursorRepo> mockSyncCursorRepo = new();
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
                    BookId = Guid.NewGuid(),
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
                    BookId = Guid.NewGuid(),
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
                    BookId = Guid.NewGuid(),
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
                    BookId = Guid.NewGuid(),
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
                    BookId = Guid.NewGuid(),
                },
            ];

            Mock<IBookApiService> bookApiBLL = new();
            Mock<ISyncCursorRepo> syncCursorRepo = new();

            BLLResponse bLLResponse = new() { Success = true, Content = apiResultBooks };
            Book? bookLastUpdate = null;

            bookApiBLL.Setup(x => x.GetByLastUpdateAsync(lastUpdate, 1)).ReturnsAsync(() => bLLResponse);
            mockBookDAL.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(bookLastUpdate);
            mockBookDAL.Setup(x => x.GetByBookIdAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync((Book?)null);
            mockBookDAL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(1);
            syncCursorRepo.Setup(x => x.GetAsync(SyncCursorKeys.Book)).ReturnsAsync(DateTime.MinValue);

            BookSyncService bookSyncBLL = new(bookApiBLL.Object, mockBookDAL.Object, syncCursorRepo.Object);

            await bookSyncBLL.ApiToLocalSync(1, lastUpdate);

            mockBookDAL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Exactly(5));
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(0));
        }

        [TestMethod()]
        public async Task ApiToLocalSync_Create_and_UpdateLocalBooksTest()
        {
            DateTime lastUpdate = DateTime.Now.AddDays(-3);

            var book1Id = Guid.NewGuid();
            var book2Id = Guid.NewGuid();

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
                    BookId = book1Id,
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
                    LocalId = 2,
                    BookId = book2Id,
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
                    LocalId = 3,
                    BookId = Guid.NewGuid(),
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
                    LocalId = 4,
                    BookId = Guid.NewGuid(),
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
                    LocalId = 5,
                    BookId = Guid.NewGuid(),
                },
            ];

            // Local books that exist and should be updated (their UpdatedAt is older than API)
            Book localBook1 = new()
            {
                Title = "Teste de Título old",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-6),
                UserId = 1,
                Id = 1,
                LocalId = 1,
                BookId = book1Id,
            };

            Book localBook2 = new()
            {
                Title = "Teste de Título 2 old",
                Authors = "Emanuel Teste",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now.AddDays(-7),
                UserId = 1,
                Id = 2,
                LocalId = 2,
                BookId = book2Id,
            };

            Mock<IBookApiService> bookApiBLL = new();
            Mock<IBookRepo> mockBookDAL = new();
            Mock<ISyncCursorRepo> syncCursorRepo = new();

            BLLResponse bLLResponse = new() { Success = true, Content = apiResultBooks };

            bookApiBLL.Setup(x => x.GetByLastUpdateAsync(lastUpdate, 1)).ReturnsAsync(() => bLLResponse);
            syncCursorRepo.Setup(x => x.GetAsync(SyncCursorKeys.Book)).ReturnsAsync(lastUpdate);

            // Books 1 and 2 exist locally (matched by BookId)
            mockBookDAL.Setup(x => x.GetByBookIdAsync(book1Id, 1)).ReturnsAsync(localBook1);
            mockBookDAL.Setup(x => x.GetByBookIdAsync(book2Id, 1)).ReturnsAsync(localBook2);
            // Books 3, 4, 5 don't exist locally
            mockBookDAL.Setup(x => x.GetByBookIdAsync(It.Is<Guid>(g => g != book1Id && g != book2Id), It.IsAny<int>())).ReturnsAsync((Book?)null);
            mockBookDAL.Setup(x => x.GetByIdAsync(It.Is<int>(id => id > 2))).ReturnsAsync((Book?)null);

            mockBookDAL.Setup(x => x.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(1);
            mockBookDAL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(1);

            BookSyncService bookSyncBLL = new(bookApiBLL.Object, mockBookDAL.Object, syncCursorRepo.Object);

            await bookSyncBLL.ApiToLocalSync(1, lastUpdate);

            mockBookDAL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Exactly(3));
            mockBookDAL.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "Push pending books to server")]
        [TestMethod()]
        public async Task PushPendingAsync_PushesBooks_Test()
        {
            var book1 = new Book()
            {
                Title = "livro um",
                Authors = "autor um",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                LocalId = 2,
                BookId = Guid.NewGuid(),
                SyncStatus = BookSyncStatus.Pending,
            };

            var book2 = new Book()
            {
                Title = "livro dois",
                Authors = "autor dois",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                LocalId = 3,
                BookId = Guid.NewGuid(),
                SyncStatus = BookSyncStatus.Pending,
            };

            BLLResponse createResponse1 = new() { Success = true, Content = 100 };
            BLLResponse createResponse2 = new() { Success = true, Content = 200 };

            mockBookDAL.Setup(x => x.GetPendingPushAsync(1)).ReturnsAsync([book1, book2]);
            mockBookApiBLL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(createResponse1);

            BookSyncService bookSyncBLL = new(mockBookApiBLL.Object, mockBookDAL.Object, mockSyncCursorRepo.Object);

            await bookSyncBLL.PushPendingAsync(1);

            mockBookDAL.Verify(x => x.SetSyncStatusAsync(It.IsAny<int>(), BookSyncStatus.Pushing), Times.Exactly(2));
            mockBookDAL.Verify(x => x.SetExternalIdAndSyncedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            mockBookApiBLL.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Exactly(2));
        }
    }
}
