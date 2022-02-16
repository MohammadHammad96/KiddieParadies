using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KiddieParadies.Core.Models;
using KiddieParadies.Data;
namespace KiddieParadies.Helpers
{
    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, int>
    {
        public ApplicationUserStore(ApplicationDbContext context)
            : base(context)
        {
            AutoSaveChanges = false;
        }
    }
}