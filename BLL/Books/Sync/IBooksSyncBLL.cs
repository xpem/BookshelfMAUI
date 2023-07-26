namespace BookshelfServices.Books.Sync
{
    public interface IBooksSyncBLL
    {
        void StartThread();

        Timer? Timer { get; set; }
        
        bool ThreadIsRunning { get; set; }
    }
}
