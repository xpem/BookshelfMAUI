namespace Services
{
    public interface IBuildDbBLL
    {
        Task CleanLocalDatabase();
        void Init();
    }
}