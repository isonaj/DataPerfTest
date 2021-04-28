using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPerfTest.Models
{
    public class MyDbContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<LearningItem> LearningItems { get; set; }
        public DbSet<LearningPlan> LearningPlans { get; set; }
        public DbSet<LearningHistory> LearningHistories { get; set; }

        public DbSet<FactTable> Facts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=myDb;User id=sa;Password=P@ssw0rd!;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().HasKey(p => p.Id);
            modelBuilder.Entity<LearningItem>().HasKey(p => p.Id);

            modelBuilder.Entity<FactTable>().HasKey(p => new { p.PersonId, p.LearningItemId });
            modelBuilder.Entity<LearningHistory>().HasKey(p => new { p.PersonId, p.LearningItemId });
            modelBuilder.Entity<LearningPlan>().HasKey(p => new { p.PersonId, p.LearningItemId });

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.LearningPlans)
                .WithOne()
                .HasForeignKey(p => p.PersonId);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.LearningHistories)
                .WithOne()
                .HasForeignKey(p => p.PersonId);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Facts)
                .WithOne()
                .HasForeignKey(p => p.PersonId);


            modelBuilder.Entity<LearningItem>()
                .HasMany(p => p.LearningPlans)
                .WithOne()
                .HasForeignKey(p => p.LearningItemId);

            modelBuilder.Entity<LearningItem>()
                .HasMany(p => p.LearningHistories)
                .WithOne()
                .HasForeignKey(p => p.LearningItemId);

            modelBuilder.Entity<LearningItem>()
                .HasMany(p => p.Facts)
                .WithOne()
                .HasForeignKey(p => p.LearningItemId);

        }
    }
}
