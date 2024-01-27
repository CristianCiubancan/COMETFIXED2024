using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("account_user_roles")]
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        [Key]
        [ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId")]
        public override int UserId { get; set; }
        [Key]
        [ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId")]
        public override int RoleId { get; set; }
        [Column] public DateTime CreationDate { get; set; }
        [Column] public DateTime? ModifiedDate { get; set; }
    }
}
