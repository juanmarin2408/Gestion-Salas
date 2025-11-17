using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Sala> Salas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Equipo> Equipos { get; set; }

    }

}
