using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for User aggregate root.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all users.
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Adds a new user.
        /// </summary>
        Task AddAsync(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        void Update(User user);

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}