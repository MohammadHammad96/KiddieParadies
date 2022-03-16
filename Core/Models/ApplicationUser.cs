using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace KiddieParadies.Core.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<int>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<int>> Tokens { get; set; }

        public ApplicationUser()
        {
            //UserRoles = new List<ApplicationUserRole>();
        }
    }

    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public virtual ApplicationRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    /*public class manager : UserManager<ApplicationUser>
    {
        public manager() : base()
        {
            this.rol
        }
    }*/
}