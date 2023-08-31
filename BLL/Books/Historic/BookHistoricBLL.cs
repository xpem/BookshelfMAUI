﻿using BLL.Books.Historic.Interfaces;
using BLL.User;
using DBContextDAL;
using Models.Books.Historic;
using Microsoft.EntityFrameworkCore;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL : IBookHistoricBLL
    {
        private readonly IUserBLL userBLL;
        private readonly BookshelfDbContext bookshelfDbContext;

        public BookHistoricBLL(IUserBLL userBLL,BookshelfDbContext bookshelfDbContext)
        {
            this.userBLL = userBLL;
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public BookHistoricList GetBookHistoricByBookId(int? page, int bookId)
        {
            List<BookHistoric> list = new();

            int total = 0;

            Models.User? User = userBLL.GetUserLocal().Result;

            if (User?.Id != null)
            {
                int pageSize = 10;

                list = bookshelfDbContext.BookHistoric.Where(x => x.Uid == User.Id && x.BookId == bookId)
                .Include(x => x.BookHistoricItems).OrderByDescending(x => x.CreatedAt).ToList();

                total = list.Count;

                if (page != null)
                    list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();
            }

            return new BookHistoricList() { List = list, Total = total };
        }
    }
}
