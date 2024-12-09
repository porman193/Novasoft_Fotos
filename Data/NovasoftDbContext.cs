using CasaToro.Novasoft.Fotos.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaToro.Novasoft.Fotos.Data
{
    public class NovasoftDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración del contexto
        public NovasoftDbContext(DbContextOptions<NovasoftDbContext> options) : base(options)
        {

        }

        // DbSet para la entidad Employee
        public required DbSet<Employee> Employees { get; set; }

        // DbSet para la entidad BusinessUnit
        public required DbSet<BusinessUnit> BusinessUnits { get; set; }

        // Configuración del modelo mediante Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la clave primaria para la entidad Employee
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.idEmployee);

            // Configuración de la clave primaria para la entidad BusinessUnit
            modelBuilder.Entity<BusinessUnit>()
                .HasKey(e => e.code);

            base.OnModelCreating(modelBuilder);
        }
    }
}
