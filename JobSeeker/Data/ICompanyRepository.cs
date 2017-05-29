using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Models;
using JobSeeker.Models.Company;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JobSeeker.Data
{
    public interface ICompanyRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAllAsync();

        // Companies
        IEnumerable<Company> GetAllCompanies();
        Company GetCompany(int id);

        IEnumerable<Job> GetAllJobs(int companyId);
        Job GetJob(int id, int companyId);
    }
}
