using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("account_roles")]
    public class ApplicationRole : IdentityRole<int>
    {
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }

        public DateTime CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
