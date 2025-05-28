using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for CompanyLink aggregate root.
    /// </summary>
    public interface ICompanyLinkRepository
    {
        /// <summary>
        /// Gets a company link by its unique identifier.
        /// </summary>
        Task<CompanyLink?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all company links.
        /// </summary>
        Task<IEnumerable<CompanyLink>> GetAllAsync();

        /// <summary>
        /// Adds a new company link.
        /// </summary>
        Task AddAsync(CompanyLink link);

        /// <summary>
        /// Updates an existing company link.
        /// </summary>
        void Update(CompanyLink link);

        /// <summary>
        /// Deletes a company link by its unique identifier.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}