using System.Collections.Generic;
using System.Threading.Tasks;
using ScaffoldDeneme.Models;

namespace ScaffoldDeneme.Repositories
{
    public interface INationalityRepository : IRepository<Nationality>
    {
        Task<IEnumerable<Nationality>> GetNationalitiesOrderedAsync();
        Task<IEnumerable<object>> GetNationalitiesByLengthAsync();
    }
}
