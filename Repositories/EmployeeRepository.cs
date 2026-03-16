using FlexFit.Data;
using FlexFit.Models;
using FlexFit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlexFit.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetByIdAsync(int id) =>
            await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

        // Dodata metoda u tvom stilu
        public async Task<Employee> GetByEmailAsync(string email) =>
            await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

        public async Task<IEnumerable<Employee>> GetAllAsync() =>
            await _context.Employees.ToListAsync();

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}