using BookshelfRepos.BuildDb;

namespace BookshelfServices
{
    public static class BuildDbBLL
    {
        public static void BuildSQLiteDb() => BuildDbRepos.BuildDb().Wait();
    }
}
