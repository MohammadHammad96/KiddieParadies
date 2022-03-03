using KiddieParadies.Core.Models;
using KiddieParadies.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KiddieParadies.Core.Helpers
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