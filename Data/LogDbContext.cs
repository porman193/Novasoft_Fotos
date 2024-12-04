using Microsoft.EntityFrameworkCore;
using CasaToro.Novasoft.Fotos.Models;

namespace CasaToro.Novasoft.Fotos.Data
{
    public class LogDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración del contexto
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }
        // DbSet para la entidad Log
        public DbSet<Log> Logs { get; set; }

        // DbSet para la entidad Card
        public DbSet<Card> Cards { get; set; }
        // Configuración del modelo mediante Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la clave primaria para la entidad Log
            modelBuilder.Entity<Log>()
                .HasKey(e => e.idLog);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>()
                .HasKey(e => e.idEmployee);
            base.OnModelCreating(modelBuilder);
        }
    }
}
