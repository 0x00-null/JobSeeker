using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JobSeeker.Models;
using JobSeeker.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JobSeeker.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Manage Accounts")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityController> _logger;
        private readonly IPasswordHasher<ApplicationUser> _hasher;
        private readonly IConfigurationRoot _config;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityController> logger,
            IPasswordHasher<ApplicationUser> hasher,
            IConfigurationRoot config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _hasher = hasher;
            _config = config;
        }

        // Get All Users
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var role = await _roleManager.FindByNameAsync("user");
                var users = await _userManager.GetUsersInRoleAsync(role.Name);

                return new JsonResult(users);
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not get all users, Ex: {e}");
            }

            return BadRequest("Could not get all users");
        }

        // Delete User
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var result = await _userManager.DeleteAsync(user);
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not delete a user, Ex: {e}");
            }

            return BadRequest("Could not delete a user");
        }

        // Create User
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = new ApplicationUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    AccessFailedCount = 0,
                    EmailConfirmed = false,
                    LockoutEnabled = true,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.UserName.ToUpper(),
                    TwoFactorEnabled = false
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    await AddToRole(model.UserName, "user");
                    await AddClaims(model.UserName, "user");
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors.ToArray());
                }

                return new JsonResult(result.Errors);
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not create a new user, Ex: {e}");
            }

            return BadRequest("Could not create a new user");
        }

        #region Helper Methods

        private async Task AddToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task AddClaims(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = new List<Claim>
            {
                new Claim(type: "role", value: role),
                new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email)
            };

            await _userManager.AddClaimsAsync(user, claims);
        }

        #endregion

        // Generate JWT Token
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> CreateToken([FromBody]CredentialModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) ==
                        PasswordVerificationResult.Success)
                    {
                        var userClaims = await _userManager.GetClaimsAsync(user);

                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email)
                        }.Union(userClaims);

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"],
                            audience: _config["Tokens:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(60),
                            signingCredentials: creds
                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
                else
                {
                    return BadRequest(Json("Credentials are incorrect").Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while logging in: {ex}");
            }

            return BadRequest("Failed to generate token");
        }
    }
}
