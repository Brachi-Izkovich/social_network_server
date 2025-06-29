using Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class MessageDto
    {
        public string Content { get; set; }
        //public DateTime TimeSend { get; set; }
        //public int? UserId { get; set; }
        public int TopicId { get; set; }
    }
}
