using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Comet.Web.App.Services;
using Comet.Web.App.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Comet.Web.App.Services.Security;

namespace Comet.Web.App.API
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = Moderator)]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext mDbContext;
        private readonly UserManager mUserManager;
        private readonly LanguageService mLanguageService;

        public RolesController(ApplicationDbContext dbContext, UserManager userManager, LanguageService language)
        {
            mDbContext = dbContext;
            mUserManager = userManager;
            mLanguageService = language;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.HasPermission(Claims.ViewRoles))
                return Forbid();

            var result = await mDbContext.Roles.ToListAsync();
            List<object> resultList = new();
            foreach (var res in result)
            {
                resultList.Add(new
                {
                    res.Id,
                    Name = mLanguageService.GetString(res.Name),
                    res.NormalizedName,
                    res.CreationDate,
                    res.ModifiedDate,
                    Count = await mDbContext.UserRoles.CountAsync(x => x.RoleId == res.Id)
                });
            }
            return Ok(resultList);
        }

        [HttpGet(Name = "GetUsersByRole")]
        public async Task<IActionResult> GetUsersByRoleAsync(int roleId, int page = 0, int ipp = 10)
        {
            var userRoles = await mDbContext.UserRoles.Where(x => x.RoleId == roleId).Select(x => x.UserId).ToArrayAsync();
            var result = await mDbContext.Users.Where(x => userRoles.Any(y => y == x.Id))
                .Select(user => new
                {
                    user.Id,
                    user.Name,
                    user.UserName
                })
                .Skip(page)
                .Take(Math.Max(1, ipp))
                .ToListAsync();
            return Ok(result);
        }

        [HttpGet(Name = "GetClaimsByRole")]
        public async Task<IActionResult> GetClaimsByRoleAsync(int roleId)
        {
            var claims = await mDbContext.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync();

            var result = new List<object>();
            foreach (var claim in claims)
            {
                result.Add(new
                {
                    claim.Id,
                    ClaimType = claim.ClaimType.Split(new char[] { '.' }, 2)[1],
                    claim.ClaimValue,
                    ClaimName = (Enum.TryParse<Claims>(claim.ClaimType, out var eClaim) ? mLanguageService.GetString(eClaim.ToPermissionString()) : mLanguageService.GetString(claim.ClaimType))
                });
            }
            return Ok(result);
        }

        [Authorize(Policy = Administrator)]
        [HttpPost(Name = "SetClaimToRole")]
        public async Task<IActionResult> SetClaimToRoleAsync([FromBody] RoleClaimModel model)
        {
            if (!Enum.TryParse(model.claimType, out Claims claimType))
                return BadRequest("Invalid claim type.");

            var roleClaim = await mDbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == model.roleId && x.ClaimType.Equals(claimType.ToPermissionString()));
            if (roleClaim != null)
                return Ok();

            if (model.claimType.Equals(Claims.Super.ToString()))
                return Forbid();

            roleClaim = new ApplicationRoleClaim
            {
                ClaimType = claimType.ToPermissionString(),
                ClaimValue = "1",
                RoleId = model.roleId
            };
            mDbContext.RoleClaims.Update(roleClaim);
            await mDbContext.SaveChangesAsync();
            return Ok(roleClaim);
        }

        [Authorize(Policy = Administrator)]
        [HttpDelete(Name = "DeleteClaimFromRole")]
        public async Task<IActionResult> DeleteClaimFromRoleAsync([FromBody] RoleClaimModel model)
        {
            if (!Enum.TryParse(model.claimType, out Claims claimType))
                return BadRequest("Invalid claim type.");

            var roleClaim = await mDbContext.RoleClaims.FirstOrDefaultAsync(x => x.RoleId == model.roleId && x.ClaimType.Equals(claimType.ToPermissionString()));
            if (roleClaim == null)
                return NotFound($"Claim '{model.claimType}' not found for the selected role.");

            if (roleClaim.ClaimType.Equals(Claims.Super.ToString()))
                return Forbid();

            mDbContext.RoleClaims.Remove(roleClaim);
            await mDbContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = Administrator)]
        [HttpPost(Name = "SetUserOnRole")]
        public async Task<IActionResult> SetUserOnRoleAsync([FromBody] RoleUserModel model)
        {
            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(model.userName))
            {
                user = await mUserManager.FindByNameAsync(model.userName);
            }
            else if (!string.IsNullOrEmpty(model.email))
            {
                user = await mUserManager.FindByEmailAsync(model.userName);
            }
            else
            {
                user = await mUserManager.FindByIdAsync(model.userId.ToString());
            }

            if (user == null)
                return NotFound("The selected user could not be found.");

            var roles = await mUserManager.GetRolesAsync(user);
            if (roles.Any(x => x.Equals(Roles.SuperAdministrator)) && !User.IsInRole(Roles.SuperAdministrator))
                return Forbid("The selected user is an Super Administrator and cannot be changed.");

            var selectedRole = await mDbContext.Roles.FirstOrDefaultAsync(x => x.Id == model.roleId);
            if (selectedRole == null)
                return NotFound("The selected role does not exist.");

            if (selectedRole.Name.Equals(Roles.SuperAdministrator) && !User.IsInRole(Roles.SuperAdministrator))
                return Forbid("You cannot set users on Super Administrator level.");

            if (roles.Any(x => x.Equals(selectedRole.Name)))
                return Conflict("You are trying to add the user on it's own role.");

            if (roles.Count > 0)
            {
                await mUserManager.RemoveFromRolesAsync(user, roles);
            }

            if ((await mUserManager.AddToRoleAsync(user, selectedRole.Name)) == IdentityResult.Success)
                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                });

            return BadRequest();
        }

        [Authorize(Policy = Administrator)]
        [HttpPost("DeleteUserFromRole")]
        public async Task<IActionResult> DeleteUserFromRoleAsync(RoleUserModel model)
        {
            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(model.userName))
            {
                user = await mUserManager.FindByNameAsync(model.userName);
            }
            else if (!string.IsNullOrEmpty(model.email))
            {
                user = await mUserManager.FindByEmailAsync(model.userName);
            }
            else
            {
                user = await mUserManager.FindByIdAsync(model.userId.ToString());
            }

            if (user == null)
                return NotFound("The selected user could not be found.");

            var roles = await mUserManager.GetRolesAsync(user);
            if (roles.Any(x => x.Equals(Roles.SuperAdministrator)) && !User.IsInRole(Roles.SuperAdministrator))
                return Forbid("The selected user is an Super Administrator and cannot be changed.");

            var selectedRole = await mDbContext.Roles.FirstOrDefaultAsync(x => x.Id == model.roleId);
            if (selectedRole == null)
                return NotFound("The selected role does not exist.");

            if (await mUserManager.RemoveFromRoleAsync(user, selectedRole.Name) == IdentityResult.Success)
                return Ok();

            return BadRequest();
        }

        public class RoleClaimModel
        {
            public int roleId { get; set; }
            public string claimType { get; set; }
        }

        public class RoleUserModel
        {
            public int roleId { get; set; }
            public string userName { get; set; }
            public string email { get; set; }
            public int userId { get; set; }
        }
    }
}
