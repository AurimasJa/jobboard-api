using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static jobboard.Data.Enums;

namespace jobboard
{
    public class JobBoardDbContext : IdentityDbContext<JobBoardUser>
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<CompaniesResume> CompaniesResumes { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Requirements> Requirements { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<JobResumes> JobResumes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=jobboard;Trusted_Connection=True;Encrypt=False;");

            optionsBuilder.UseSqlServer("Server=DESKTOP-DR4U6PP\\MSSQLSERVER01;Database=jobboard;Trusted_Connection=True;Encrypt=False;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //        modelBuilder.Entity<Education>()
            //.Property(e => e.Degree)
            //.HasConversion<int>();
            modelBuilder
                .Entity<Education>()
                .Property(e => e.Degree)
                .HasConversion(
                    v => v.ToString(),
                    v => (Degree)Enum.Parse(typeof(Degree), v));
        }
    }
}
