using Microsoft.EntityFrameworkCore;
using DelivAssist.Models;

namespace DelivAssist.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<UserDelivery> UserDeliveries { get; set; }
        public DbSet<UserExpense> UserExpenses { get; set; }
        public DbSet<UserShift> UserShifts { get; set; }
        public DbSet<ShiftDelivery> ShiftDeliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDelivery>()
                .HasKey(ud => new { ud.UserId, ud.DeliveryId });

            modelBuilder.Entity<UserDelivery>()
                .HasOne(ud => ud.User)
                .WithMany(ud => ud.UserDeliveries)
                .HasForeignKey(ud => ud.UserId);

            modelBuilder.Entity<UserDelivery>()
                .HasOne(ud => ud.Delivery)
                .WithMany(ud => ud.UserDeliveries)
                .HasForeignKey(ud => ud.DeliveryId);

            modelBuilder.Entity<UserExpense>()
                .HasKey(ue => new { ue.UserId, ue.ExpenseId });

            modelBuilder.Entity<UserExpense>()
                .HasOne(ue => ue.User)
                .WithMany(ue => ue.UserExpenses)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<UserExpense>()
                .HasOne(ue => ue.Expense)
                .WithMany(ue => ue.UserExpenses)
                .HasForeignKey(ue => ue.ExpenseId);

            modelBuilder.Entity<UserShift>()
                .HasKey(us => new { us.UserId, us.ShiftId });

            modelBuilder.Entity<UserShift>()
                .HasOne(us => us.User)
                .WithMany(us => us.UserShifts)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserShift>()
                .HasOne(us => us.Shift)
                .WithMany(us => us.UserShifts)
                .HasForeignKey(us => us.ShiftId);

            modelBuilder.Entity<ShiftDelivery>()
                .HasKey(sd => new { sd.ShiftId, sd.DeliveryId });

            modelBuilder.Entity<ShiftDelivery>()
                .HasOne(sd => sd.Shift)
                .WithMany(sd => sd.ShiftDeliveries)
                .HasForeignKey(sd => sd.ShiftId);

            modelBuilder.Entity<ShiftDelivery>()
                .HasOne(sd => sd.Delivery)
                .WithMany(sd => sd.ShiftDeliveries)
                .HasForeignKey(sd => sd.DeliveryId);
        }
    }
}