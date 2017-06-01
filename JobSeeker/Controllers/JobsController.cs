using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Data;
using JobSeeker.Models;
using JobSeeker.ViewModels.Job;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JobSeeker.Controllers
{
    [Route("api/companies")]
    [Authorize]
    public class JobsController : Controller
    {
        private readonly ILogger<JobsController> _logger;
        private readonly ICompanyRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsController(
            ILogger<JobsController> logger,
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

        // Get a single job by ID from company
        [HttpGet("{companyId}/jobs/{id}", Name = "GetJob")]
        [AllowAnonymous]
        public IActionResult Get(int companyId, int id)
        {
            try
            {
                var job = _repo.GetJob(id, companyId);
                if (job == null) return NotFound($"Job could not be found within in company id {companyId} with an id of {id}");
                return Ok(job);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch this specific job. Ex: {e}");
            }

            return BadRequest($"Failed to fetch this job ID {id} in the company ID of {companyId}");
        }

        // Get all jobs from company
        [HttpGet("{companyId}/jobs")]
        [AllowAnonymous]
        public IActionResult GetAll(int companyId)
        {
            try
            {
                var jobs = _repo.GetAllJobs(companyId);
                if (jobs == null && !jobs.Any()) return NotFound($"Could not find any jobs in the company of ID {companyId}");
                return Ok(jobs);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch all jobs in the company of ID {companyId}. Ex: {e}");
            }

            return BadRequest($"Failed to fetch any jobs in the company of ID {companyId}");
        }

        // Create a new job 
        [HttpPost("{companyId}/jobs")]
        public async Task<IActionResult> Post(int companyId, [FromBody] JobViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var company = _repo.GetCompanyForUser(companyId, userName);
                if (company == null) return NotFound("No companies found to create this job for");

                var job = new Job()
                {
                    Title = model.Title,
                    JobTags = model.JobTags,
                    Location = model.Location,
                    Description = model.Description,
                    SkillsAndRequirements = model.SkillsAndRequirements,
                    Salary = model.Salary,
                    CompanyId = model.CompanyId
                };

                company.Jobs.Add(job);

                var newUri = Url.Link("GetJob", job);

                if (await _repo.SaveAllAsync())
                {
                    return Created(newUri, job);
                }
                else
                {
                    return BadRequest("Failed to create a new job");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create a new job. Exception: {e}");
            }

            return BadRequest("Failed to create a new job");
        }
    }
}
