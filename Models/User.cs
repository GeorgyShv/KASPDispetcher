using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace KASPDispetcher.Models
{
    public class User : IdentityUser
    {
        public int DepartmentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }

        public virtual Подразделение Department { get; set; }

        public virtual ICollection<Master> Masters { get; set; }
    }
}
