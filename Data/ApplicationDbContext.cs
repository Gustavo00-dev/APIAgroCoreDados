using APIAgroCoreDados.Model;
using Microsoft.EntityFrameworkCore;

namespace APIAgroCoreDados.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Propriedade> Propriedades { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<Talhao> Talhao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Propriedade>().HasKey(p => p.IdPropriedade);
            modelBuilder.Entity<Talhao>().HasKey(t => t.IdTalhao);
            modelBuilder.Entity<Sensor>().HasKey(s => s.Id);
        }
    }
}
