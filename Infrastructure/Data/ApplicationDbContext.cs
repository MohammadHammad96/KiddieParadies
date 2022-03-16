using KiddieParadies.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KiddieParadies.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUserRole> AspNetUserRoles { get; set; }

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

        public DbSet<CourseClassRoom> CourseClassRooms { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            //builder.Entity<ApplicationUserRole>().HasKey(p => new { p.UserId, p.RoleId });
            builder.Entity<ApplicationUserRole>().ToTable("AspNetUserRoles");

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

            builder.Entity<CourseClassRoom>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<CourseClassRoom>()
                .HasIndex(c => new { c.ClassRoom, c.Day, c.Order, c.CourseId })
                .IsUnique();

            builder.Entity<StudentCourse>()
                .HasOne(s => s.Student)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            /*builder.Entity<StudentCourse>().HasOne(s => s.Course)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);*/
            builder.Entity<StudentCourse>().HasIndex(sc => new { sc.StudentId, sc.CourseId }).IsUnique();

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
