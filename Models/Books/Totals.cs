namespace Models.Books
{
    public class Totals
    {
        public int IllRead { get; set; }//1

        public int Reading { get; set; }//2

        public int Read { get; set; }//3

        public int Interrupted { get; set; }//4       
    }

    public class TotalBooksGroupedByStatus
    {
        public int Count { get; set; }

        public Models.Books.Status? Status
        {
            get; set;
        }
    }
}
