using Microsoft.EntityFrameworkCore;

namespace DBContextDAL
{
    public class BookshelfDbContext : DbContext
    {
        public virtual DbSet<Models.VersionDbTables> VersionDbTables { get; set; }

        public virtual DbSet<Models.User> User { get; set; }

        public virtual DbSet<Models.Books.Book> Book { get; set; }

        public virtual DbSet<Models.Books.Historic.BookHistoric> BookHistoric { get; set; }

        public virtual DbSet<Models.Books.Historic.BookHistoricItem> BookHistoricItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db")}");
        }
    }
}