using BookshelfRepos.BuildDb;

namespace BLL
{
    public static class BuildDbBLL
    {
        public static void BuildSQLiteDb() => BuildLocalDbDAL.BuildDb().Wait();
    }
}
