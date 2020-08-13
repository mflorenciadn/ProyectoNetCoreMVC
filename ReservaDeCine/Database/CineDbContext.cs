using Microsoft.EntityFrameworkCore;
using MVC.ReservaDeCine.Models;
using System.Collections;

namespace MVC.ReservaDeCine.Database
{
    public class CineDbContext : DbContext
    {

        public CineDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<TipoSala> TiposSala { get; set; }
        public DbSet<Clasificacion> Clasificaciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public IEnumerable Roles { get; internal set; }

    }
}
