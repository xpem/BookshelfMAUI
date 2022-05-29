using BookshelfRepos.BuildDb;

namespace BookshelfServices
{
    public static class BuildDbServices
    {
        public static void BuildSQLiteDb() => BuildDbRepos.BuildDb();
    }
}
