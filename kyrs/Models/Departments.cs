using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrs.Models
{
    public class Departments
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public ICollection<Employees> Employees { get; set; }
    }
}
