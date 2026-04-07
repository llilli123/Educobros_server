using Microsoft.EntityFrameworkCore;
using EduCobros.Models;

namespace EduCobros.Data
{
    public class EduCobrosContext : DbContext
    {
        public EduCobrosContext(DbContextOptions<EduCobrosContext> options)
            : base(options) { }

        // Cada DbSet = una tabla en la BD
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        // Seed Data: datos iniciales
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estudiante>().HasData(
                new Estudiante { Id = 1, Nombre = "María García Pérez", Grado = "4to Primaria", Mensualidad = 4500, MesesDebidos = 0 },
                new Estudiante { Id = 2, Nombre = "Carlos Rodríguez", Grado = "6to Primaria", Mensualidad = 4500, MesesDebidos = 2 },
                new Estudiante { Id = 3, Nombre = "Ana Martínez Díaz", Grado = "2do Sec.", Mensualidad = 5200, MesesDebidos = 0 },
                new Estudiante { Id = 4, Nombre = "Luis Hernández", Grado = "3ro Sec.", Mensualidad = 5200, MesesDebidos = 4 },
                new Estudiante { Id = 5, Nombre = "Sofía Reyes Guzmán", Grado = "1ro Primaria", Mensualidad = 3800, MesesDebidos = 0 }
            );
        }
    }
}