using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrs.Models
{
    public class Employees
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Age { get; set; }
        public double Money { get; set; }
        public DateTime StartWork { get; set; }
        public Departments Department { get; set; }
        public Positions Position { get; set; } 

    }
}
