﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Books;
using Models.DTOs;
using Models.Responses;
using Moq;
using Repos;
using Repos.Interfaces;
using Services.Books;
using Services.Books.Interfaces;
using Xunit;

namespace BLLTests.Books
{
    [TestClass()]
    public class BookBLLTests
    {
        Mock<IBookApiService> bookApiBLL = new();
        Mock<IBookRepo> mockBookDAL = new();
        Mock<IBooksOperationService> mockBooksOperationBLL = new();

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

            Mock<IBookApiService> bookApiBLL = new();
            Mock<IBookRepo> mockBookDAL = new();

            mockBookDAL.Setup(x => x.GetTotalBooksGroupedByStatusAsync(1)).ReturnsAsync(totalBooksGroupedByStatuses);

            Mock<IUserRepo> mockUserDAL = new();
            Mock<IBooksOperationService> mockBooksOperationBLL = new();

            BookService booksBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockBooksOperationBLL.Object);

            Totals? result = booksBLL.GetBookshelfTotalsAsync(1).Result;

            if (result is not null && result.IllRead == 1 && result.Reading == 2 && result.Read == 3 && result.Interrupted == 0)
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }

        [Fact(DisplayName = "Retorna resultado de livro já criado com este mesmo id no bd local")]
        [TestMethod()]
        public void UpdateBook_FaiL_Validation_Unique_Title_Test()
        {
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

            Mock<IBookApiService> bookApiBLL = new();
            Mock<IBookRepo> mockBookDAL = new();
            Mock<IBooksOperationService> mockBooksOperationBLL = new();

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

            mockBookDAL.Setup(x => x.CheckIfExistsWithSameTitleAsync(1, "Teste de Título 6", 2)).ReturnsAsync(true);
            mockBookDAL.Setup(x => x.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(1);

            BLLResponse bLLResponse = new() { Success = true };

            bookApiBLL.Setup(x => x.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(bLLResponse);


            BookService booksBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockBooksOperationBLL.Object);

            BLLResponse? result = booksBLL.UpdateAsync(1, true, UptBook).Result;

            if (result is not null && result.Success == false && result.Content is not null && (string)result.Content == "Livro com este título já cadastrado.")
                Assert.IsTrue(true);
            else
                Assert.Fail();
        }


        [Fact(DisplayName = "Cria um livro local e insere uma operação de criação deste na fila")]
        [TestMethod()]
        public void AddBookAsync_NoConnection_InsertOperation_Test()
        {
            Book insBook = new()
            {
                Title = "Teste de Título Insert",
                Authors = "Emanuel Teste Insert",
                Status = Status.IllRead,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                Id = 2,
                LocalId = 2
            };

            Book? returnBook = null;
            BLLResponse bLLResponse = new() { Success = true, Content = 2 };

            mockBookDAL.Setup(x => x.GetByTitleOrGoogleIdAsync(1, "Teste de Título Insert", null)).ReturnsAsync(returnBook);
            mockBookDAL.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(1);

            Mock<BookService> booksBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockBooksOperationBLL.Object);

            BLLResponse? result = booksBLL.Object.AddAsync(1, false, insBook).Result;

            mockBooksOperationBLL.Verify(x => x.InsertOperationInsertBookAsync(It.IsAny<Book>()), Times.Once());

            Assert.IsTrue(result?.Success);
        }

        [Fact(DisplayName = "Atualiza um livro local e insere uma operação de atualização deste na fila")]
        [TestMethod()]
        public void UpdateBookAsync_NoConnection_InsertOperation_Test()
        {

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
            mockBookDAL.Setup(x => x.CheckIfExistsWithSameTitleAsync(1, "Teste de Título 6", 2)).ReturnsAsync(false);
            mockBookDAL.Setup(x => x.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(1);

            BLLResponse bLLResponse = new() { Success = true };

            bookApiBLL.Setup(x => x.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(bLLResponse);

            Mock<BookService> booksBLL = new(bookApiBLL.Object, mockBookDAL.Object, mockBooksOperationBLL.Object);

            BLLResponse? result = booksBLL.Object.UpdateAsync(1, false, UptBook).Result;

            mockBooksOperationBLL.Verify(x => x.InsertOperationUpdateBookAsync(UptBook), Times.Once());

            Assert.IsTrue(result?.Success);
        }
    }
}