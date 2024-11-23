using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repositories
{
    public class UserRepo(BookshelfDbContext bookshelfDbContext) : IUserRepo
    {
        public async Task<User?> GetUserLocalAsync() => await bookshelfDbContext.User.FirstOrDefaultAsync();

        //public Task<int> GetUidAsync() => bookshelfDbContext.User.Select(x => x.Id).FirstAsync();

        public async Task<int> CreateAsync(User user)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            await bookshelfDbContext.User.AddAsync(user);
            return await bookshelfDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            await bookshelfDbContext.User.Where(x => x.Id == user.Id).ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Email, user.Email)
            .SetProperty(z => z.Name, user.Name)
            .SetProperty(z => z.Password, user.Password)
            .SetProperty(z => z.Token, user.Token));
        }

        public int UpdateLastUpdate(DateTime lastUpdate, int uid)
        {
            bookshelfDbContext.ChangeTracker.Clear();

            return bookshelfDbContext.User.Where(x => x.Id == uid).ExecuteUpdate(y => y.SetProperty(z => z.LastUpdate, lastUpdate));

        }

    }
}
