namespace BLL
{
    public interface IBuildDbBLL
    {
        Task CleanLocalDatabase();
        Task Init();
    }
}