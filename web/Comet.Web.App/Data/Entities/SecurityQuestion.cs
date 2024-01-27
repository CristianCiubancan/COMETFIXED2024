using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Web.App.Data.Entities
{
    [Table("security_questions")]
    public class SecurityQuestion
    {
        [Key]
        public int Id {  get; set; }
        public string Name { get; set; }
    }
}
