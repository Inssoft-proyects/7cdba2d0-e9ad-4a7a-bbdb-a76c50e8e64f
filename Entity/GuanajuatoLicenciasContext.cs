using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class GuanajuatoLicenciasContext : DbContext
    {
        public GuanajuatoLicenciasContext(DbContextOptions<GuanajuatoLicenciasContext> options) : base(options) { }
         
        public DbSet<LicenciaPersonaDatos> personaDatos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenciaPersonaDatos>().HasNoKey();
        }
    }
}
