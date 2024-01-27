using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Comet.Web.App.Areas.Panel.Pages.Admin
{
    [Authorize]
    public class RolesModel : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}
