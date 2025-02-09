using System;
using System.Threading.Tasks;
using ScaffoldDeneme.Models;

namespace ScaffoldDeneme.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        INationalityRepository Nationalities { get; }
        IStudentRepository Students { get; }

        Task<int> CompleteAsync(); 
    }
}
