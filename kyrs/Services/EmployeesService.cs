using kyrs.Data;
using kyrs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kyrs.Services
{
    public class EmployeesService
    {
        private readonly AppDbContext _context;

        public EmployeesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employees>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .ToListAsync();
        }

        public async Task AddEmployeeAsync(Employees employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employees employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Departments>> GetDepartmentsWithMostEmployeesAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .OrderByDescending(d => d.Employees.Count)
                .ToListAsync();
        }

        public async Task<List<Employees>> GetTop5OldestEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .OrderBy(e => e.Age)
                .Take(5)
                .ToListAsync();
        }

        public async Task<Dictionary<string, double>> GetAverageSalaryByDepartmentAsync()
        {
            var result = await _context.Employees
                .Include(e => e.Department)
                .GroupBy(e => e.Department.Name)
                .Select(g => new
                {
                    DepartmentName = g.Key,
                    AverageSalary = g.Average(e => e.Money)
                })
                .ToListAsync();

            return result.ToDictionary(x => x.DepartmentName, x => x.AverageSalary);
        }
        // Отримати всі відділи для випадаючого списку
        public async Task<List<Departments>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        // Отримати всі посади для випадаючого списку
        public async Task<List<Positions>> GetAllPositionsAsync()
        {
            return await _context.Positions.ToListAsync();
        }
    }
}