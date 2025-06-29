using Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class TopicDto
    {
        public string Title { get; set; }
        //public int? UserId { get; set; }
        public int? CategoryId { get; set; }
    }
}
