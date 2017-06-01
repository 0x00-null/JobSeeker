using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Data;
using JobSeeker.Models;
using JobSeeker.Models.Company;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JobSeeker.Helpers
{
    public class EnsureSeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyRepository _companyRepo;

        public EnsureSeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ICompanyRepository companyRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyRepo = companyRepo;
        }

        public async Task ExecuteSeed()
        {

            // Create new "super" user
            var testUser = await _userManager.FindByNameAsync("tester");
            if (testUser == null)
            {
                var newUser = new ApplicationUser()
                {
                    FirstName = "Peter",
                    LastName = "Kujawski",
                    Email = "elusiven@me.com",
                    EmailConfirmed = true,
                    UserName = "tester"
                };

                await _userManager.CreateAsync(newUser, "test");

                // Create administrator role and assign permission to manage accounts
                var adminRole = await _roleManager.FindByNameAsync("administrator");
                if (adminRole == null)
                {
                    adminRole = new IdentityRole("administrator");
                    await _roleManager.CreateAsync(adminRole);
                }

                var userRole = await _roleManager.FindByNameAsync("user");
                if (userRole == null)
                {
                    userRole = new IdentityRole("user");
                    await _roleManager.CreateAsync(userRole);
                }

                if (!await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync("tester"), adminRole.Name))
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync("tester"), adminRole.Name);
                }

                await _userManager.AddClaimAsync(await _userManager.FindByNameAsync("tester"),
                    new Claim(ClaimTypes.Role, "administrator"));

            }

            // Create example company.
            var company = _companyRepo.GetCompany(0);
            if (company == null)
            {
                var newCompany = new Company()
                {
                    Name = "English Language School",
                    UserName = "tester",
                    Description = "This is a testing description",
                    AboutUs = "",
                    ImageUrl = "",
                    Type = "Private",
                    YearFounded = new DateTime(2017),
                    Size = new CompanySize() { Min = 5, Max = 15 },
                    WebsiteUrl = "www.englishlanguage-school.co.uk"
                };

                _companyRepo.Add(newCompany);
                await _companyRepo.SaveAllAsync();
            }
        }
    }
}
