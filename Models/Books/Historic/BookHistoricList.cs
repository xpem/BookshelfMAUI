using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Books.Historic
{
    public class BookHistoricList
    {
        public required List<BookHistoric> List { get; set; }

        public int Total { get; set; }
    }
}
