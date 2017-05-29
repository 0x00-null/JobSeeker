using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Models;
using JobSeeker.Models.Company;
using Microsoft.AspNetCore.Http;

namespace JobSeeker.Data
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public IEnumerable<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        public Company GetCompany(int id)
        {
            return _context.Companies.FirstOrDefault(c => c.CompanyId == id);
        }

        public IEnumerable<Job> GetAllJobs(int companyId)
        {
            return _context.Jobs.Where(c => c.CompanyId == companyId);
        }

        public Job GetJob(int id, int companyId)
        {
            return _context.Jobs.Where(j => j.JobId == id).FirstOrDefault(j => j.CompanyId == companyId);
        }
    }
}
