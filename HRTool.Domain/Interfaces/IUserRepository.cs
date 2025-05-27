using System;
using System.Collections.Generic;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for User aggregate root.
    /// </summary>
    public interface IUserRepository
    {
        User? GetById(Guid id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(Guid id);
    }
}