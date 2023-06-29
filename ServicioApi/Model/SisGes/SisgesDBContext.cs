using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ServicioApi.Model.SisGes
{
    public partial class SisgesDBContext : DbContext
    {
        public SisgesDBContext()
        {
        }

        public SisgesDBContext(DbContextOptions<SisgesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auditorium> Auditoria { get; set; } = null!;
        public virtual DbSet<CentrosLocal> CentrosLocals { get; set; } = null!;
        public virtual DbSet<DetalleSencillo> DetalleSencillos { get; set; } = null!;
        public virtual DbSet<EstadoSencillo> EstadoSencillos { get; set; } = null!;
        public virtual DbSet<IdocFi> IdocFis { get; set; } = null!;
        public virtual DbSet<IdocPagosFi> IdocPagosFis { get; set; } = null!;
        public virtual DbSet<InterfazAutorizacion> InterfazAutorizacions { get; set; } = null!;
        public virtual DbSet<InterfazPago> InterfazPagos { get; set; } = null!;
        public virtual DbSet<MontoSencillo> MontoSencillos { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<Remito> Remitos { get; set; } = null!;
        public virtual DbSet<Sencillo> Sencillos { get; set; } = null!;
        public virtual DbSet<SencillosSap> SencillosSaps { get; set; } = null!;
        public virtual DbSet<SencillosTienda> SencillosTiendas { get; set; } = null!;
        public virtual DbSet<TipoInterfaz> TipoInterfazs { get; set; } = null!;
        public virtual DbSet<TipoTransaccionCapa> TipoTransaccionCapas { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Venta> Ventas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=StringConexionSisges");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auditorium>(entity =>
            {
                entity.HasKey(e => new { e.IdUsuario, e.Fecha });

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Descripcion).HasColumnType("text");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Auditoria)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Auditoria_Usuario");
            });

            modelBuilder.Entity<CentrosLocal>(entity =>
            {
                entity.HasKey(e => e.CenCodigo);

                entity.ToTable("CENTROS_LOCAL");

                entity.Property(e => e.CenCodigo)
                    .ValueGeneratedNever()
                    .HasColumnName("CEN_Codigo");

                entity.Property(e => e.CenCodigoBanco)
                    .HasMaxLength(14)
                    .HasColumnName("CEN_CodigoBanco");

                entity.Property(e => e.CenCuentaBanco)
                    .HasMaxLength(24)
                    .HasColumnName("CEN_CuentaBanco");

                entity.Property(e => e.CenEstado)
                    .HasMaxLength(1)
                    .HasColumnName("CEN_estado")
                    .IsFixedLength();

                entity.Property(e => e.CenNombre)
                    .HasMaxLength(35)
                    .HasColumnName("CEN_Nombre");

                entity.Property(e => e.CenPos).HasColumnName("CEN_Pos");

                entity.Property(e => e.CenServidor)
                    .HasMaxLength(255)
                    .HasColumnName("CEN_Servidor");

                entity.Property(e => e.Correo)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DetalleSencillo>(entity =>
            {
                entity.ToTable("DetalleSencillo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Banco)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DiaEntrega)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DiaLiberacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEntrega).HasColumnType("date");

                entity.Property(e => e.FechaLiberacion).HasColumnType("date");

                entity.Property(e => e.IdSencillosSap).HasColumnName("IdSencillosSAP");

                entity.HasOne(d => d.IdSencilloNavigation)
                    .WithMany(p => p.DetalleSencillos)
                    .HasForeignKey(d => d.IdSencillo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DetalleSencillo_Sencillos");

                entity.HasOne(d => d.IdSencillosSapNavigation)
                    .WithMany(p => p.DetalleSencillos)
                    .HasForeignKey(d => d.IdSencillosSap)
                    .HasConstraintName("FK_DetalleSencillo_SencillosSAP");

                entity.HasOne(d => d.IdTiendaNavigation)
                    .WithMany(p => p.DetalleSencillos)
                    .HasForeignKey(d => d.IdTienda)
                    .HasConstraintName("FK_DetalleSencillo_CENTROS_LOCAL");
            });

            modelBuilder.Entity<EstadoSencillo>(entity =>
            {
                entity.HasKey(e => e.Codigo);

                entity.ToTable("EstadoSencillo");

                entity.Property(e => e.Codigo).ValueGeneratedNever();

                entity.Property(e => e.Descripcion).HasColumnType("text");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sigla)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<IdocFi>(entity =>
            {
                entity.HasKey(e => new { e.Fechaint, e.Newbs, e.Wrbtr, e.Sgtxt, e.Werks });

                entity.ToTable("IDOC_FI");

                entity.Property(e => e.Fechaint)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("FECHAINT");

                entity.Property(e => e.Newbs)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBS");

                entity.Property(e => e.Wrbtr)
                    .HasColumnType("money")
                    .HasColumnName("WRBTR");

                entity.Property(e => e.Sgtxt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SGTXT");

                entity.Property(e => e.Werks)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WERKS");

                entity.Property(e => e.Aufnr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("AUFNR");

                entity.Property(e => e.Bktxt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BKTXT");

                entity.Property(e => e.Blart)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BLART");

                entity.Property(e => e.Bldat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("BLDAT");

                entity.Property(e => e.Budat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("BUDAT");

                entity.Property(e => e.Bukrs)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BUKRS");

                entity.Property(e => e.Bupla)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BUPLA");

                entity.Property(e => e.Correlat).HasColumnName("CORRELAT");

                entity.Property(e => e.Ebeln)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("EBELN");

                entity.Property(e => e.Fwbas)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("FWBAS");

                entity.Property(e => e.Gsber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("GSBER");

                entity.Property(e => e.Interfaz)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("INTERFAZ");

                entity.Property(e => e.KostlPrctr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("KOSTL_PRCTR");

                entity.Property(e => e.Kursf)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("KURSF");

                entity.Property(e => e.Mandt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MANDT");

                entity.Property(e => e.Mansp)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MANSP");

                entity.Property(e => e.Menge)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MENGE");

                entity.Property(e => e.Monat)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("MONAT");

                entity.Property(e => e.Mwskz)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MWSKZ");

                entity.Property(e => e.Name1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NAME1");

                entity.Property(e => e.Newbk)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBK");

                entity.Property(e => e.Newbw)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBW");

                entity.Property(e => e.Newko)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NEWKO");

                entity.Property(e => e.Newum)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWUM");

                entity.Property(e => e.Nitem).HasColumnName("NITEM");

                entity.Property(e => e.Ort01)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ORT01");

                entity.Property(e => e.Prctr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("PRCTR");

                entity.Property(e => e.Skfbt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("SKFBT");

                entity.Property(e => e.Stcd1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("STCD1");

                entity.Property(e => e.Valut)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("VALUT");

                entity.Property(e => e.Vbund)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("VBUND");

                entity.Property(e => e.Waers)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WAERS");

                entity.Property(e => e.Wwert)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WWERT");

                entity.Property(e => e.Xblnr).HasColumnName("XBLNR");

                entity.Property(e => e.Xmwst)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XMWST");

                entity.Property(e => e.Xref1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF1");

                entity.Property(e => e.Xref2)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF2");

                entity.Property(e => e.Xref3)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF3");

                entity.Property(e => e.Zfbdt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZFBDT");

                entity.Property(e => e.Zlspr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZLSPR");

                entity.Property(e => e.Zterm)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZTERM");

                entity.Property(e => e.Zuonr)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("ZUONR");
            });

            modelBuilder.Entity<IdocPagosFi>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("IDOC_PAGOS_FI");

                entity.Property(e => e.Aufnr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("AUFNR");

                entity.Property(e => e.Bktxt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BKTXT");

                entity.Property(e => e.Blart)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BLART");

                entity.Property(e => e.Bldat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("BLDAT");

                entity.Property(e => e.Budat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("BUDAT");

                entity.Property(e => e.Bukrs)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BUKRS");

                entity.Property(e => e.Bupla)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("BUPLA");

                entity.Property(e => e.Correlat).HasColumnName("CORRELAT");

                entity.Property(e => e.Ebeln)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("EBELN");

                entity.Property(e => e.Fechaint)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("FECHAINT");

                entity.Property(e => e.Fwbas)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("FWBAS");

                entity.Property(e => e.Gsber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("GSBER");

                entity.Property(e => e.Interfaz)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("INTERFAZ");

                entity.Property(e => e.KostlPrctr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("KOSTL_PRCTR");

                entity.Property(e => e.Kursf)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("KURSF");

                entity.Property(e => e.Mandt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MANDT");

                entity.Property(e => e.Mansp)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MANSP");

                entity.Property(e => e.Menge)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("MENGE");

                entity.Property(e => e.Monat)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("MONAT");

                entity.Property(e => e.Mwskz)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MWSKZ");

                entity.Property(e => e.Name1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NAME1");

                entity.Property(e => e.Newbk)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBK");

                entity.Property(e => e.Newbs)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBS");

                entity.Property(e => e.Newbw)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWBW");

                entity.Property(e => e.Newko)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NEWKO");

                entity.Property(e => e.Newum)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("NEWUM");

                entity.Property(e => e.Nitem).HasColumnName("NITEM");

                entity.Property(e => e.Ort01)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ORT01");

                entity.Property(e => e.Prctr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("PRCTR");

                entity.Property(e => e.Sgtxt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SGTXT");

                entity.Property(e => e.Skfbt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("SKFBT");

                entity.Property(e => e.Stcd1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("STCD1");

                entity.Property(e => e.Valut)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("VALUT");

                entity.Property(e => e.Vbund)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("VBUND");

                entity.Property(e => e.Waers)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WAERS");

                entity.Property(e => e.Werks)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WERKS");

                entity.Property(e => e.Wrbtr)
                    .HasColumnType("money")
                    .HasColumnName("WRBTR");

                entity.Property(e => e.Wwert)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("WWERT");

                entity.Property(e => e.Xblnr).HasColumnName("XBLNR");

                entity.Property(e => e.Xmwst)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XMWST");

                entity.Property(e => e.Xref1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF1");

                entity.Property(e => e.Xref2)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF2");

                entity.Property(e => e.Xref3)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("XREF3");

                entity.Property(e => e.Zfbdt)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZFBDT");

                entity.Property(e => e.Zlspr)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZLSPR");

                entity.Property(e => e.Zterm)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ZTERM");

                entity.Property(e => e.Zuonr)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("ZUONR");
            });

            modelBuilder.Entity<InterfazAutorizacion>(entity =>
            {
                entity.HasKey(e => new { e.FechaAutorizacion, e.CodigoAgencia, e.NumeroAutorizacion });

                entity.ToTable("InterfazAutorizacion");

                entity.HasIndex(e => new { e.CodigoAgencia, e.Estado }, "<IndexReporteCAAU, sysname,>");

                entity.HasIndex(e => new { e.CodigoCliente, e.Monto, e.NroTrxPos }, "IndexReporteCAAU2");

                entity.Property(e => e.FechaAutorizacion).HasColumnType("datetime");

                entity.Property(e => e.CodigoAgencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroAutorizacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoEmpresa)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoUsuario)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Confirmada)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Estado)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Extrafinanciamiento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaAutoriza).HasColumnType("date");

                entity.Property(e => e.FechaPrimerVencimiento).HasColumnType("datetime");

                entity.Property(e => e.FechaUltimoVencimiento).HasColumnType("datetime");

                entity.Property(e => e.ModoProceso)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NroRef)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroBoleta)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroTarjeta)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RutVendedor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoArchivo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDiferencia)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TipoTransaccion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InterfazPago>(entity =>
            {
                entity.HasKey(e => new { e.FechaAutorizacion, e.CodigoAgencia, e.NumeroAutorizacion });

                entity.ToTable("InterfazPago");

                entity.Property(e => e.FechaAutorizacion).HasColumnType("datetime");

                entity.Property(e => e.CodigoAgencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroAutorizacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoEmpresa)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoUsuario)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NroRef)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Numerocuenta)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Numerorecibo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Referencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoArchivo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDiferencia)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.CodigoTransaccionNavigation)
                    .WithMany(p => p.InterfazPagos)
                    .HasForeignKey(d => d.CodigoTransaccion)
                    .HasConstraintName("FK_InterfazPago_TipoTransaccionCAPA");
            });

            modelBuilder.Entity<MontoSencillo>(entity =>
            {
                entity.ToTable("MontoSencillo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdDatalleSencilloNavigation)
                    .WithMany(p => p.MontoSencillos)
                    .HasForeignKey(d => d.IdDatalleSencillo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MontoSencillo_DetalleSencillo");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.ToTable("Persona");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ApellidoMaterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ApellidoPaterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(95)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([varchar](95),(((rtrim(ltrim([ApellidoPaterno]))+' ')+isnull(rtrim(ltrim([ApellidoMaterno])),''))+', ')+rtrim(ltrim([Nombres])),(0)))", false);

                entity.Property(e => e.Nombres)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Run)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(CONVERT([varchar],([dbo].[FormatInt]([RunCuerpo])+'-')+[RunDigito],(0)))", false);

                entity.Property(e => e.RunDigito)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Remito>(entity =>
            {
                entity.HasKey(e => new { e.Codigo, e.IdTienda });

                entity.ToTable("Remito");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdTienda).HasColumnName("idTienda");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.IdSencillosTienda).HasColumnName("idSencillosTienda");

                entity.Property(e => e.NumeroDepostio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSencillosTiendaNavigation)
                    .WithMany(p => p.Remitos)
                    .HasForeignKey(d => d.IdSencillosTienda)
                    .HasConstraintName("FK_Remito_Sencillos_Tiendas");

                entity.HasOne(d => d.IdTiendaNavigation)
                    .WithMany(p => p.Remitos)
                    .HasForeignKey(d => d.IdTienda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Remito_CENTROS_LOCAL");
            });

            modelBuilder.Entity<Sencillo>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.FechaFin).HasColumnType("datetime");

                entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            });

            modelBuilder.Entity<SencillosSap>(entity =>
            {
                entity.ToTable("SencillosSAP");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Asignacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cla)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.FechaDoc)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Doc");

                entity.Property(e => e.ImporMl).HasColumnName("ImporML");

                entity.Property(e => e.Io)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("IO");

                entity.Property(e => e.LibMayor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Lib_Mayor");

                entity.Property(e => e.NDoc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("N_doc");

                entity.Property(e => e.Referencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RutProvedor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Texto)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SencillosTienda>(entity =>
            {
                entity.ToTable("Sencillos_Tiendas");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Creacion");

                entity.Property(e => e.IdDetalleSencillo).HasColumnName("idDetalleSencillo");

                entity.Property(e => e.IdTienda).HasColumnName("idTienda");

                entity.HasOne(d => d.CodigoEstadoSencilloNavigation)
                    .WithMany(p => p.SencillosTienda)
                    .HasForeignKey(d => d.CodigoEstadoSencillo)
                    .HasConstraintName("FK_Sencillos_Tiendas_EstadoSencillo");

                entity.HasOne(d => d.IdDetalleSencilloNavigation)
                    .WithMany(p => p.SencillosTienda)
                    .HasForeignKey(d => d.IdDetalleSencillo)
                    .HasConstraintName("FK_Sencillos_Tiendas_DetalleSencillo");

                entity.HasOne(d => d.IdTiendaNavigation)
                    .WithMany(p => p.SencillosTienda)
                    .HasForeignKey(d => d.IdTienda)
                    .HasConstraintName("FK_Sencillos_Tiendas_CENTROS_LOCAL");
            });

            modelBuilder.Entity<TipoInterfaz>(entity =>
            {
                entity.HasKey(e => e.Codigo);

                entity.ToTable("TipoInterfaz");

                entity.Property(e => e.Codigo).ValueGeneratedNever();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoTransaccionCapa>(entity =>
            {
                entity.HasKey(e => e.Codigo);

                entity.ToTable("TipoTransaccionCAPA");

                entity.Property(e => e.Codigo).ValueGeneratedNever();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FechaCreacion)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasMaxLength(130)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Usuario)
                    .HasForeignKey<Usuario>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Persona");
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => new { e.Trx, e.Local, e.FechaVenta });

                entity.ToTable("VENTAS");

                entity.HasIndex(e => new { e.Local, e.TipoDocumento }, "Index_boletascontraloria2");

                entity.HasIndex(e => e.TipoDocumento, "index_boletascontraloria");

                entity.Property(e => e.Trx).HasColumnName("TRX");

                entity.Property(e => e.FechaVenta)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Venta");

                entity.Property(e => e.Bruto).HasColumnType("money");

                entity.Property(e => e.Caja2).HasMaxLength(12);

                entity.Property(e => e.CajaFueraLinea).HasColumnName("Caja_Fuera_Linea");

                entity.Property(e => e.Cajero).HasMaxLength(12);

                entity.Property(e => e.Cliente).HasMaxLength(24);

                entity.Property(e => e.CodigoAnula).HasColumnName("Codigo_Anula");

                entity.Property(e => e.FechaVentaLinea)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Venta_Linea");

                entity.Property(e => e.FechaVtaSistema)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Vta_Sistema");

                entity.Property(e => e.FolioDocumento)
                    .HasMaxLength(30)
                    .HasColumnName("Folio_Documento");

                entity.Property(e => e.Impto).HasColumnType("money");

                entity.Property(e => e.ModoEntrenamiento).HasColumnName("Modo_Entrenamiento");

                entity.Property(e => e.Neto).HasColumnType("money");

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(15)
                    .HasColumnName("Tipo_Documento");

                entity.Property(e => e.TipoImpto).HasColumnName("Tipo_Impto");

                entity.Property(e => e.TipoTrx).HasColumnName("Tipo_Trx");

                entity.Property(e => e.TotalVenta)
                    .HasColumnType("money")
                    .HasColumnName("Total_Venta");

                entity.Property(e => e.TrxImpreso).HasColumnName("TRX_Impreso");

                entity.Property(e => e.Vendedor).HasMaxLength(24);

                entity.Property(e => e.VentaIniciada).HasColumnName("Venta_Iniciada");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
