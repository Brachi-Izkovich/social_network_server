using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class SearchResultDto
    {
        public string Type { get; set; } // "Topic" or "Message"
        public int Id { get; set; }
        public string TitleOrContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
    }

}
