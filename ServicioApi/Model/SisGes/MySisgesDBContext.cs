using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Mysqlx.Cursor;
using ServicioApi.Model.Clases;
using ServicioApi.Modelview;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;


namespace ServicioApi.Model.SisGes
{
    public class MySisgesDbcontext : SisgesDBContext
    {
        private readonly IConfiguration _config;


        public MySisgesDbcontext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {           

            optionsBuilder.UseSqlServer(_config.GetConnectionString("StringConexionSisges"));


        }
        public DbSet<SencillosTesoreria> fnSencillosTiendaNoConciliados { get; set; }
        public DbSet<FormatoIDOC> FormatoIDOCResults { get; set; }

        public DbSet<IdocVentasRP> VentasIDOCResults { get; set; }

        public DbSet<IdocNCFC_RP> NCFCIDOCResults { get; set; }
        
        public DbSet<FormatoIDOC> MPVentasIDOCResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<SencillosTesoreria>().HasNoKey().ToFunction("fnSencillosTiendaNoConciliados");

            modelBuilder.Entity<FormatoIDOC>().HasNoKey().ToView(null);

            modelBuilder.Entity<IdocVentasRP>().HasNoKey().ToView(null);

            modelBuilder.Entity<IdocNCFC_RP>().HasNoKey().ToView(null);
        }



        [DbFunction("fnSencillosTiendaNoConciliados", "dbo")]
        public static IQueryable<SencillosTesoreria> fnSencillosTiendaNoConciliados_Result()
        {
            // Este método no tiene cuerpo porque EF Core no lo ejecuta directamente.
            throw new Exception("No se puede invocar este método directamente");
        }

        public List<FormatoIDOC> fn_IdocClientes_RP(int local, string Fecha, string Ambiente)
        {
            // Se utiliza la función de base de datos "fn_IdocClientes_RP" y los resultados se almacenan en FormatoIDOCResults.
            var results = FormatoIDOCResults.FromSqlRaw("SELECT * FROM dbo.fn_IdocClientes_RP({0}, {1}, {2})", local, Fecha, Ambiente).ToList();
            return results;
        }

        public List<IdocVentasRP> fn_IdocVentasRP(int local, string Fecha, string Ambiente)
        {
            // Se utiliza la función de base de datos "fn_IdocClientes_RP" y los resultados se almacenan en FormatoIDOCResults.
            var results = VentasIDOCResults.FromSqlRaw("SELECT * FROM dbo.fn_IdocVentasRP({0}, {1}, {2})", local, Fecha, Ambiente).ToList();
            return results;
        }

        public List<IdocNCFC_RP> fn_IdocNCFC_RP(int local, string Fecha, string Ambiente)
        {
            // Se utiliza la función de base de datos "fn_IdocClientes_RP" y los resultados se almacenan en FormatoIDOCResults.
            var results = NCFCIDOCResults.FromSqlRaw("SELECT * FROM dbo.fn_IdocNCFC_RP({0}, {1}, {2})", local, Fecha, Ambiente).ToList();
            return results;
        }

        public List<FormatoIDOC> fn_IdocMPVentas_RP(int local, string Fecha, string Ambiente)
        {
            // Se utiliza la función de base de datos "fn_IdocMPVentas" y los resultados se almacenan en MPVentasIDOCResults.
            var results = MPVentasIDOCResults.FromSqlRaw("SELECT * FROM dbo.fn_IdocMPVentas({0}, {1}, {2})", local, Fecha, Ambiente).ToList();
            return results;
        }
    }
}
