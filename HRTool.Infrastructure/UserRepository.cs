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
    /// Repository implementation for User aggregate root.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly HrDbContext _context;

        public UserRepository(HrDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.Include(u => u.Manager).FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Manager).ToListAsync();
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
    }
}
