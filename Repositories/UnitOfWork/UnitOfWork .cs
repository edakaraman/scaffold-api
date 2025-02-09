using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using ScaffoldDeneme.Models;
using Microsoft.Extensions.Logging;

namespace ScaffoldDeneme.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MydatabaseContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private INationalityRepository? _nationalities;
        private IStudentRepository? _students;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(MydatabaseContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public INationalityRepository Nationalities
        {
            get
            {
                return _nationalities ??= new NationalityRepository(_context);
            }
        }

        public IStudentRepository Students
        {
            get
            {
                return _students ??= new StudentRepository(_context);
            }
        }

        public async Task<int> CompleteAsync()
        {
            try
            {                
                if (_currentTransaction == null)
                {
                    _currentTransaction = await _context.Database.BeginTransactionAsync();
                    _logger.LogInformation("Transaction started.");
                }

                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("Changes saved to the database.");

                await _currentTransaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes to the database.");

                if (_currentTransaction != null)
                {
                    try
                    {
                        await _currentTransaction.RollbackAsync();
                        _logger.LogInformation("Transaction rolled back.");
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "An error occurred while rolling back the transaction.");
                    }
                }

                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _logger.LogInformation("Transaction disposed.");
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing UnitOfWork.");
            _context.Dispose();
        }
    }
}
