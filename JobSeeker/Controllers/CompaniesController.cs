using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Data;
using JobSeeker.Models;
using JobSeeker.Models.Company;
using JobSeeker.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JobSeeker.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ILogger<CompaniesController> _logger;
        private readonly ICompanyRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(
            ILogger<CompaniesController> logger,
            ICompanyRepository repo,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        #region CRUD Actions

        // Get a single company by ID
        [HttpGet("{id}", Name = "GetCompany")]
        [AllowAnonymous]
        public IActionResult Get(int id)
        {
            try
            {
                var company = _repo.GetCompany(id);
                if (company == null) return NotFound($"Could not find a company with ID of {id}");
                return Ok(company);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch a company. Ex: {e}");
            }

            return BadRequest($"Failed to fetch a company with ID of {id}");
        }

        // Get all companies
        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                var companies = _repo.GetAllCompanies();
                if (companies == null && !companies.Any()) return NotFound("Could not find any companies");
                return Ok(companies);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch any companies. Ex: {e}");
            }

            return BadRequest("Failed to fetch all companies");
        }

        // Create a company
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newCompany = new Company()
                {
                    Name = model.Name,
                    ImageUrl = model.ImageUrl,
                    Description = model.Description,
                    AboutUs = model.AboutUs,
                    Type = model.Type,
                    CompanyAdress = model.CompanyAdress,
                    WebsiteUrl = model.WebsiteUrl,
                    Size = model.Size
                };

                _repo.Add(newCompany);

                var newUri = Url.Link("GetCompany", newCompany);

                if (await _repo.SaveAllAsync())
                {
                    return Created(newUri, newCompany);
                }
                else
                {
                    return BadRequest("Failed to add a new company");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add a new company. Exception: {e}");
            }

            return BadRequest("Faield to add a new company");
        }

        // Edit a company


        // Delete a company
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var company = _repo.GetCompany(id);
                if (company == null) return NotFound($"Company with id of {id} not found");

                if (GetCurrentUser().Result.UserName == company.UserName)
                {
                    _repo.Delete(company);
                }
                else
                {
                    return Unauthorized();
                }
                
                if (await _repo.SaveAllAsync())
                {
                    return Ok($"The company with id of {id} has been deleted");
                }
                else
                {
                    return BadRequest($"Failed to delete a company with id of {id}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete a company with id of {id}. Exception: {e}");
            }

            return BadRequest($"Failed to delete a company with id of {id}");
        }
        #endregion
    }
}
