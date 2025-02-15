using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Microsoft.EntityFrameworkCore.Internal;
using Repos.Interfaces;

namespace Repos
{
    public class UserRepo(IDbContextFactory<BookshelfDbContext> bookshelfDbContext) : IUserRepo
    {
        public async Task<User?> GetUserLocalAsync()
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.User.FirstOrDefaultAsync();
        }

        //public Task<int> GetUidAsync() => bookshelfDbContext.User.Select(x => x.Id).FirstAsync();

        public async Task<int> CreateAsync(User user)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            await context.User.AddAsync(user);
            return await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            using var context = bookshelfDbContext.CreateDbContext();

            await context.User.Where(x => x.Id == user.Id).ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Email, user.Email)
            .SetProperty(z => z.Name, user.Name)
            .SetProperty(z => z.Password, user.Password)
            .SetProperty(z => z.Token, user.Token));
        }

        public int UpdateLastUpdate(DateTime lastUpdate, int uid)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return context.User.Where(x => x.Id == uid).ExecuteUpdate(y => y.SetProperty(z => z.LastUpdate, lastUpdate));

        }

    }
}
