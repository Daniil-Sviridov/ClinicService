using Microsoft.EntityFrameworkCore;

namespace ClinicService.Data
{
    public class ClinicServiceDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Consultation> Consultations { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountSession> AccountSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Consultation>().HasOne(p => p.Pet).WithMany(b => b.Consultations).HasForeignKey(p => p.PetId)
                .OnDelete(DeleteBehavior.NoAction);

        }

        public ClinicServiceDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
