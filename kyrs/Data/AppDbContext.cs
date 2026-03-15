using kyrs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace kyrs.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Positions> Positions { get; set; } 
        public DbSet<Employees> Employees { get; set; }
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options)
        {

        }
    }

}
