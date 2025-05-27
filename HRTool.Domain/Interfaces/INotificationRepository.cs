using System;
using System.Collections.Generic;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Notification aggregate root.
    /// </summary>
    public interface INotificationRepository
    {
        Notification? GetById(Guid id);
        IEnumerable<Notification> GetAll();
        void Add(Notification notification);
        void Update(Notification notification);
        void Delete(Guid id);
    }
}