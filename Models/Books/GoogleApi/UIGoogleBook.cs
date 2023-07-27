using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Books.GoogleApi
{
    public class UIGoogleBook
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Authors { get; set; }

        public string Publisher { get; set; }
        
        public string PublishedDate { get; set; }

        public int PageCount { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public string Categories { get; set; }
    }
}
