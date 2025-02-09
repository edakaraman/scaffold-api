using Microsoft.EntityFrameworkCore;
using ScaffoldDeneme.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScaffoldDeneme.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(MydatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
        {
            return await _context.Students.Where(s => s.IsActive).ToListAsync();
        }

    
        public async Task<IEnumerable<Student>> GetStudentsSortedByNameAsync()
        {
            return await _context.Students.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctStudentNamesAsync()
        {
            return await _context.Students
                .Select(s => s.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByAgeOrNameAsync(int year, string name)
        {
            return await _context.Students
                .Where(s => s.BirthDate.Year == year || s.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentFullNamesAsync()
        {
            return await _context.Students
                .Select(s => new Student
                {
                    Name = $"{s.Name} {s.SurName}"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsOrderNameAsync()
        {
            return await _context.Students
                .OrderByDescending(s => s.Name)
                .Select(s => s)
                .ToListAsync();
        }
    }
}
