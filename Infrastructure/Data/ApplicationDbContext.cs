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

        public DbSet<Employee> Employees { get; set; }

        public DbSet<YearEmployee> YearEmployees { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Level> Levels { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<LevelCourse> LevelCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Applicationpage>().HasIndex(p => p.Title).IsUnique();

            builder.Entity<Year>().HasIndex(y => y.Name).IsUnique();

            builder.Entity<Parent>().HasIndex(p => p.UserId).IsUnique();
            builder.Entity<Parent>().HasIndex(p => p.FatherIdentityImageName).IsUnique();
            builder.Entity<Parent>().HasIndex(p => p.MotherIdentityImageName).IsUnique();

            builder.Entity<Employee>().HasIndex(e => e.UserId).IsUnique();
            builder.Entity<Employee>().HasIndex(e => e.ImageName).IsUnique();
            builder.Entity<Employee>().HasIndex(e => e.ResumeName).IsUnique();
            builder.Entity<YearEmployee>().HasIndex(p => new { p.YearId, p.EmployeeId }).IsUnique();

            builder.Entity<YearStudent>().HasIndex(p => new { p.YearId, p.StudentId }).IsUnique();

            builder.Entity<Student>().HasIndex(s => new { s.FirstName, s.ParentId });
            builder.Entity<Student>().HasIndex(s => s.ImageName).IsUnique();

            builder.Entity<Certificate>().HasIndex(c => c.ImageName).IsUnique();

            builder.Entity<Course>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Level>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Level>().HasIndex(c => c.Order).IsUnique();

            builder.Entity<LevelCourse>().HasIndex(l => new { l.YearId, l.CourseId, l.LevelId }).IsUnique();
        }
    }
}
