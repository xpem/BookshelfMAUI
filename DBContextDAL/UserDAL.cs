using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DbContextDAL
{
    public class UserDAL(BookshelfDbContext bookshelfDbContext) : IUserDAL
    {
        public async Task<Models.User?> GetUserLocal() => await bookshelfDbContext.User.FirstOrDefaultAsync();

        public int GetUid() => bookshelfDbContext.User.Select(x => x.Id).First();

        public async Task<int> ExecuteAddUser(User user)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            await bookshelfDbContext.User.AddAsync(user);
            return await bookshelfDbContext.SaveChangesAsync();
        }

        public int ExecuteUpdateLastUpdateUser(DateTime lastUpdate, int uid)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            return bookshelfDbContext.User.Where(x => x.Id == uid).ExecuteUpdate(y => y.SetProperty(z => z.LastUpdate, lastUpdate));

        }

    }
}
