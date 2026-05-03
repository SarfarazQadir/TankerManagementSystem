using Microsoft.EntityFrameworkCore;

namespace TankerManagementSystem.Models
{
    public class TankerDbContext : DbContext
    {
        public DbSet<Admin> tbl_admin { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tanker> Tankers { get; set; }
        public DbSet<TankerOwner> TankerOwners { get; set; }
        public DbSet<TripLedger> TripLedgers { get; set; }
        public DbSet<TripExpense> TripExpenses { get; set; }
        public DbSet<CashLedger> CashLedgers { get; set; }
        public DbSet<PersonalKhata> PersonalKhatas { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CommissionSetup> CommissionSetups { get; set; }
        public DbSet<AppModule> AppModules { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<TripEntry> TripEntries { get; set; }

        public TankerDbContext(DbContextOptions<TankerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  UNIQUE constraint
            modelBuilder.Entity<UserPermission>()
                .HasIndex(up => new { up.UserId, up.ModuleId })
                .IsUnique();
        }
    }
}
