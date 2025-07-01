using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.AdminPages
{
    public class AdminChangeRoleDto
    {
        public string AdminCode { get; set; }
        public int UserId { get; set; }
        public Role NewRole { get; set; }
    }
}
