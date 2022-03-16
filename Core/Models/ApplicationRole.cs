using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace KiddieParadies.Core.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationRole()
        {
            UserRoles = new List<ApplicationUserRole>();
        }
    }
}