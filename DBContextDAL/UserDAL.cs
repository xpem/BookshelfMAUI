using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbContextDAL
{
    public class UserDAL : IUserDAL
    {
        private readonly BookshelfDbContext bookshelfDbContext;

        public UserDAL(BookshelfDbContext bookshelfDbContext)
        {
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public async Task<Models.User?> GetUserLocal() => await bookshelfDbContext.User.FirstOrDefaultAsync();

        public int GetUid() => bookshelfDbContext.User.Select(x => x.Id).First();

        public async Task<int> ExecuteAddUser(User user)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            await bookshelfDbContext.User.AddAsync(user);
            return await bookshelfDbContext.SaveChangesAsync();
        }

        public async Task<int> ExecuteUpdateLastUpdateUser(DateTime lastUpdate, int uid)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            bookshelfDbContext.User.Where(x => x.Id == uid).ExecuteUpdate(y => y.SetProperty(z => z.LastUpdate, lastUpdate));

            return await bookshelfDbContext.SaveChangesAsync();
        }

    }
}
