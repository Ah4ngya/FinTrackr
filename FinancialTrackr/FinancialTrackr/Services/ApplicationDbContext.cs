using Microsoft.EntityFrameworkCore;
using FinancialTrackr.Data.Models;
using Microsoft.EntityFrameworkCore.Internal;
namespace FinancialTrackr.Services
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Income { get; private set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MonthlyBudget> Budgets { get; set; }
        public DbSet<PaymentMethods> Methods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Income>()
                .HasOne(i => i.User)
                .WithMany(u => u.Incomes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories) 
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonthlyBudget>()
                .HasOne(mb => mb.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(mb => mb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonthlyBudget>()
                .HasIndex(mb => new { mb.UserId, mb.Month })
                .IsUnique();
            modelBuilder.Entity<PaymentMethods>()
                .HasOne(mb => mb.User)
                .WithMany(n => n.PaymentMethod)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
