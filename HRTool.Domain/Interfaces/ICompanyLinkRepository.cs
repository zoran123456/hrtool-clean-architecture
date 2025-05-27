using System;
using System.Collections.Generic;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for CompanyLink aggregate root.
    /// </summary>
    public interface ICompanyLinkRepository
    {
        CompanyLink? GetById(Guid id);
        IEnumerable<CompanyLink> GetAll();
        void Add(CompanyLink link);
        void Update(CompanyLink link);
        void Delete(Guid id);
    }
}