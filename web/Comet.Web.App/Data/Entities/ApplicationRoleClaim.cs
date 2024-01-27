using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("account_role_claims")]
    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }
        [ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId")]
        public override int RoleId { get; set; }

    }
}
