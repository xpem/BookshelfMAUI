namespace BLL
{
    public interface IBuildDbBLL
    {
        Task CleanLocalDatabase();
        void Init();
    }
}