using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("account_tokens")]
    public class ApplicationUserToken : IdentityUserToken<int>
    {
        [Key]
        [ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId")]
        public override int UserId { get => base.UserId; set => base.UserId = value; }
        [Key]
        public override string LoginProvider { get => base.LoginProvider; set => base.LoginProvider = value; }
        [Key]
        public override string Name { get => base.Value; set => base.Value = value; }
    }
}
