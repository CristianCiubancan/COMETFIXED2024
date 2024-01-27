using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("account_user_claims")]
    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }
        [ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId")]
        public override int UserId { get; set; }
    }
}
