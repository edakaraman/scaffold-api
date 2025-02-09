using Microsoft.EntityFrameworkCore;
using ScaffoldDeneme.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScaffoldDeneme.Repositories
{
    public class NationalityRepository : Repository<Nationality>, INationalityRepository
    {
        public NationalityRepository(MydatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Nationality>> GetNationalitiesOrderedAsync()
        {
            return await _context.Nationalities
                .OrderBy(n => n.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetNationalitiesByLengthAsync()
        {
            return await _context.Nationalities
                .Select(n => new
                {
                    n.Name,
                    Length = n.Name.Length
                })
                .Where(n => n.Length > 5)
                .OrderByDescending(n => n.Length)
                .ToListAsync();
        }
    }
}
