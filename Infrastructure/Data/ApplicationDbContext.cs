using KiddieParadies.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KiddieParadies.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Applicationpage> Pages { get; set; }

        public DbSet<Year> Years { get; set; }

        public DbSet<StudentRegistrationInfo> StudentRegistrationsInfos { get; set; }

        public DbSet<EmployeeRegistrationInfo> EmployeeRegistrationsInfos { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Parent> Parents { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<YearStudent> YearStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Applicationpage>().HasIndex(p => p.Title).IsUnique();

            builder.Entity<Year>().HasIndex(y => y.Name).IsUnique();

            builder.Entity<Parent>().HasIndex(p => p.UserId).IsUnique();

            builder.Entity<YearStudent>().HasIndex(p => new { p.YearId, p.StudentId }).IsUnique();

            builder.Entity<Student>().HasIndex(s => new { s.FirstName, s.ParentId });
        }
    }
}
