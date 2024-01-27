using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Comet.Web.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Web.App.Areas.Panel.Pages.Admin
{
    [Authorize]
    public class RoleModel : PageModel
    {
        private readonly ApplicationDbContext mDbContext;

        public RoleModel(ApplicationDbContext dbContext)
        {
            mDbContext = dbContext;
        }

        public Dictionary<string, Security.Claims> Claims = new Dictionary<string, Security.Claims>();

        [BindProperty(SupportsGet = true)] public int RoleId { get; set; }

        public ApplicationRole Role { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Role = await mDbContext.Roles.FindAsync(RoleId);

            Claims = new Dictionary<string, Security.Claims>(Enum.GetValues<Security.Claims>().ToDictionary(x => x.ToString()));

            if (Role == null)
                return NotFound();

            return Page();
        }
    }
}
