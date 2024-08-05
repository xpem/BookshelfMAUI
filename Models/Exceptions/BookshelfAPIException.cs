namespace Models.Exceptions
{
    [Serializable]
    public class BookshelfAPIException : Exception
    {
        public BookshelfAPIException() { }

        public BookshelfAPIException(string message) : base(message) { }
    }
}
