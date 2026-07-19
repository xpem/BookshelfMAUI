namespace Services
{
    public interface IBuildDbService
    {
        Task CleanLocalDatabase();
        Task Init();
    }
}