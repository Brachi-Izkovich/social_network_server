using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public virtual List<Topic> Topics { get; set; }//virtual- בגלל שלא חייבים לעשות לכל קטגוריה דיון
    }
}
