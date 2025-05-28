using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;
using HRTool.Infrastructure.Data;

namespace HRTool.Infrastructure
{
    /// <summary>
    /// Repository implementation for CompanyLink aggregate root.
    /// </summary>
    public class CompanyLinkRepository : ICompanyLinkRepository
    {
        private readonly HrDbContext _context;

        public CompanyLinkRepository(HrDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a company link by its unique identifier.
        /// </summary>
        public async Task<CompanyLink?> GetByIdAsync(Guid id)
        {
            return await _context.CompanyLinks.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets all company links.
        /// </summary>
        public async Task<IEnumerable<CompanyLink>> GetAllAsync()
        {
            return await _context.CompanyLinks.ToListAsync();
        }

        /// <summary>
        /// Adds a new company link.
        /// </summary>
        public async Task AddAsync(CompanyLink link)
        {
            await _context.CompanyLinks.AddAsync(link);
        }

        /// <summary>
        /// Updates an existing company link.
        /// </summary>
        public void Update(CompanyLink link)
        {
            _context.CompanyLinks.Update(link);
        }

        /// <summary>
        /// Deletes a company link by its unique identifier.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var link = await _context.CompanyLinks.FindAsync(id);
            if (link != null)
            {
                _context.CompanyLinks.Remove(link);
            }
        }
    }
}
