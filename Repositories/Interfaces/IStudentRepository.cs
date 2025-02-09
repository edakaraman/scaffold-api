using System.Collections.Generic;
using System.Threading.Tasks;
using ScaffoldDeneme.Models;

namespace ScaffoldDeneme.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<IEnumerable<Student>> GetActiveStudentsAsync();
        Task<IEnumerable<Student>> GetStudentsSortedByNameAsync();
        Task<IEnumerable<string>> GetDistinctStudentNamesAsync();
        Task<IEnumerable<Student>> GetStudentsByAgeOrNameAsync(int year, string name);
        Task<IEnumerable<Student>> GetStudentFullNamesAsync();
        Task<IEnumerable<Student>> GetStudentsOrderNameAsync();    

    }
}
