using System.Threading.Tasks;
using HRTool.Domain.Interfaces;
using HRTool.Infrastructure.Data;

namespace HRTool.Infrastructure
{
    /// <summary>
    /// Unit of Work implementation for committing changes to the data store.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HrDbContext _context;

        public UnitOfWork(HrDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
