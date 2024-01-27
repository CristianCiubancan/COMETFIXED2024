using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("accounts")]
    public class ApplicationUser : IdentityUser<int>
    {
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }

        public override string NormalizedEmail 
        { 
            get => base.NormalizedEmail;
            set => base.NormalizedEmail = value;
        }

        public override string NormalizedUserName 
        { 
            get => base.NormalizedUserName;
            set => base.NormalizedUserName = value;
        }

        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public DateTime? Birthdate { get; set; }

        public ulong SecurityCode { get; set; }
        [ForeignKey("FK_SecurityQuestion")]
        public int? SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public string Salt { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}