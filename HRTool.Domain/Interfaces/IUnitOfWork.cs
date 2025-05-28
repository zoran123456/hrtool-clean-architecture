using System.Threading.Tasks;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work interface for committing changes to the data store.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commits all changes made in the current context to the database asynchronously.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
    }
}
