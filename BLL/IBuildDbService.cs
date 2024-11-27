namespace Services
{
    public interface IBuildDbService
    {
        Task CleanLocalDatabase();
        void Init();
    }
}