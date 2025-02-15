using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.DTOs.OperationQueue;

namespace Repos
{
    public class BookshelfDbContext(DbContextOptions<BookshelfDbContext> options) : DbContext(options)
    {
        public virtual required DbSet<VersionDbTables> VersionDbTables { get; set; }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Book> Book { get; set; }

        public virtual DbSet<BookHistoric> BookHistoric { get; set; }

        public virtual DbSet<BookHistoricItem> BookHistoricItem { get; set; }

        public virtual DbSet<ApiOperation> ApiOperationQueue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db")}").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}