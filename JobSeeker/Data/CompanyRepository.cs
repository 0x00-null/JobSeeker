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

        public IEnumerable<Company> GetAllCompaniesForUser(string userName)
        {
            return _context.Companies.Where(c => c.UserName == userName).ToList();
        }

        public Company GetCompanyForUser(int companyId, string userName)
        {
            return _context.Companies.Where(c => c.CompanyId == companyId).FirstOrDefault(c => c.UserName == userName);
        }

        public Company GetCompany(int id)
        {
            return _context.Companies.FirstOrDefault(c => c.CompanyId == id);
        }

        public IEnumerable<Job> GetAllJobs(int companyId)
        {
            return _context.Companies
                .FirstOrDefault(c => c.CompanyId == companyId)
                .Jobs.ToList();
        }

        public Job GetJob(int id, int companyId)
        {
            return _context.Companies
                .FirstOrDefault(c => c.CompanyId == companyId)
                .Jobs.FirstOrDefault(j => j.JobId == id);
        }
    }
}
