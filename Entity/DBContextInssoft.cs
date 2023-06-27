using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class DBContextInssoft : DbContext
{
    public DBContextInssoft()
    {
    }

    public DBContextInssoft(DbContextOptions<DBContextInssoft> options)
        : base(options)
    {
    }

    public virtual DbSet<Dependencias> Dependencias { get; set; }

    public virtual DbSet<MarcasVehiculo> MarcasVehiculos { get; set; }

    public virtual DbSet<Oficiales> Oficiales { get; set; }

    public virtual DbSet<SubmarcasVehiculo> SubmarcasVehiculos { get; set; }

    public virtual DbSet<Estatus> Estatus { get; set; }

    public virtual DbSet<Delegaciones> Delegaciones { get; set; }

    public virtual DbSet<CatColores> Colores { get; set; }

    public virtual DbSet<TipoVehiculos> TipoVehiculos { get; set; }

    public virtual DbSet<SalariosMinimos> SalariosMinimos { get; set; }

    public virtual DbSet<DiasInhabiles> DiasInhabiles { get; set; }

    public virtual DbSet<CatMunicipios> CatMunicipios { get; set; }

    public virtual DbSet<TiposCarga> TiposCarga { get; set; }

    public virtual DbSet<MotivosInfraccion> MotivosInfraccion { get; set; }

    public virtual DbSet<CatAutoridadesDisposicion> CatAutoridadesDisposicion { get; set; }

    public virtual DbSet<CatAutoridadesEntrega> CatAutoridadesEntrega { get; set; }

    public virtual DbSet<CatInstitucionesTraslado> CatInstitucionesTraslado { get; set; }

    public virtual DbSet<CatOficinasRenta> CatOficinasRenta { get; set; }

    public virtual DbSet<CatAgenciasMinisterio> CatAgenciasMinisterio { get; set; }

    public virtual DbSet<CatClasificacionAccidentes> CatClasificacionAccidentes { get; set; }

    public virtual DbSet<CatFactoresAccidentes> CatFactoresAccidentes { get; set; }

    public virtual DbSet<CatCausasAccidentes> CatCausasAccidentes { get; set; }

    public virtual DbSet<CatFactoresOpcionesAccidentes> CatFactoresOpcionesAccidentes { get; set; }

    public virtual DbSet<CatHospitales> CatHospitales { get; set; }

    public virtual DbSet<CatDelegacionesOficinasTransporte> CatDelegacionesOficinasTransporte { get; set; }

    public virtual DbSet<Infracciones> Infracciones { get; set; }







    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Data Source=inssoft-sqlserver.database.windows.net;Database=sitteg-qa;User Id=sqladmin;Password=Inss0f7#2023;Trusted_Connection=False;TrustServerCertificate=True");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dependencias>(entity =>
        {
            entity.HasKey(e => e.IdDependencia).HasName("PK__dependen__A67AC7BE849A3403");

            entity.ToTable("dependencias");

            entity.Property(e => e.IdDependencia).HasColumnName("idDependencia");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.NombreDependencia)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreDependencia");
        });

        modelBuilder.Entity<MarcasVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdMarcaVehiculo).HasName("PK__marcasVe__33AE0F9E9DCD9C8C");

            entity.ToTable("marcasVehiculos");

            entity.Property(e => e.IdMarcaVehiculo).HasColumnName("IdMarcaVehiculo");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.MarcaVehiculo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("marcaVehiculo");
            entity.Property(e => e.ModificadoPor).HasColumnName("modificadoPor");
        });

        modelBuilder.Entity<Oficiales>(entity =>
        {
            entity.HasKey(e => e.IdOficial).HasName("PK__oficiale__7BFD0DB1E280E5C4");

            entity.ToTable("oficiales");

            entity.Property(e => e.IdOficial).HasColumnName("idOficial");

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoMaterno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoPaterno");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Rango)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rango");
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");

        });

        modelBuilder.Entity<SubmarcasVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdSubmarca);

            entity.ToTable("submarcasVehiculos");

            entity.Property(e => e.IdSubmarca).HasColumnName("IdSubmarca");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
              .HasColumnType("datetime")
              .HasColumnName("fechaActualizacion");
            entity.Property(e => e.IdMarcaVehiculo).HasColumnName("IdMarcaVehiculo");
            entity.Property(e => e.NombreSubmarca)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreSubmarca");
        });

        modelBuilder.Entity<Estatus>(entity =>
        {
            entity.HasKey(e => e.estatus);

            entity.ToTable("estatus");

            entity.Property(e => e.estatus).HasColumnName("estatus");
            entity.Property(e => e.estatusDesc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estatusDesc");
        });

        modelBuilder.Entity<Delegaciones>(entity =>
        {
            entity.HasKey(e => e.IdDelegacion);

            entity.ToTable("delegaciones");
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");

            entity.Property(e => e.Delegacion).HasColumnName("Delegacion")
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Delegacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatColores>(entity =>
        {
            entity.HasKey(e => e.IdColor);

            entity.ToTable("catColores");
            entity.Property(e => e.IdColor).HasColumnName("IdColor");

            entity.Property(e => e.color).HasColumnName("color")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        modelBuilder.Entity<TipoVehiculos>(entity =>
        {
            entity.HasKey(e => e.IdTipoVehiculo);

            entity.ToTable("tiposVehiculo");



            entity.Property(e => e.IdTipoVehiculo).HasColumnName("IdTipoVehiculo");

            entity.Property(e => e.TipoVehiculo).HasColumnName("TipoVehiculo")
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<SalariosMinimos>(entity =>
        {
            entity.HasKey(e => e.IdSalario);

            entity.ToTable("salariosMinimos");



            entity.Property(e => e.IdSalario).HasColumnName("IdSalario");

            entity.Property(e => e.Area).HasColumnName("Area")
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Salario)
                .HasColumnType("float")
                .HasColumnName("Salario");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("Fecha");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<DiasInhabiles>(entity =>
        {
            entity.HasKey(e => e.idDiaInhabil);

            entity.ToTable("diasInhabiles");



            entity.Property(e => e.idDiaInhabil).HasColumnName("idDiaInhabil");

            entity.Property(e => e.fecha).HasColumnName("fecha");
            entity.Property(e => e.idMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.todosMunicipiosBool).HasColumnName("todosMunicipiosBool");
            entity.Property(e => e.todosMunicipiosDesc).HasColumnName("todosMunicipiosDesc")
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatMunicipios>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio).HasName("IdMunicipio");

            entity.ToTable("catMunicipios");

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.Municipio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");
        });
        modelBuilder.Entity<TiposCarga>(entity =>
        {
            entity.HasKey(e => e.IdTipoCarga);

            entity.ToTable("tiposCarga");
            entity.Property(e => e.IdTipoCarga).HasColumnName("IdTipoCarga");

            entity.Property(e => e.TipoCarga).HasColumnName("TipoCarga")
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<MotivosInfraccion>(entity =>
        {
            entity.HasKey(e => e.IdMotivoInfraccion);

            entity.ToTable("motivosInfraccion");
            entity.Property(e => e.IdMotivoInfraccion).HasColumnName("IdMotivoInfraccion");

            entity.Property(e => e.Nombre).HasColumnName("Nombre")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fundamento).HasColumnName("Fundamento")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CalificacionMinima).HasColumnName("CalificacionMinima");
            entity.Property(e => e.CalificacionMaxima).HasColumnName("CalificacionMaxima");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        modelBuilder.Entity<CatAutoridadesDisposicion>(entity =>
        {
            entity.HasKey(e => e.IdAutoridadDisposicion);

            entity.ToTable("catAutoridadesDisposicion");
            entity.Property(e => e.IdAutoridadDisposicion).HasColumnName("IdAutoridadDisposicion");

            entity.Property(e => e.NombreAutoridadDisposicion).HasColumnName("NombreAutoridadDisposicion")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatAutoridadesEntrega>(entity =>
        {
            entity.HasKey(e => e.IdAutoridadEntrega);

            entity.ToTable("catAutoridadesEntrega");
            entity.Property(e => e.IdAutoridadEntrega).HasColumnName("IdAutoridadEntrega");

            entity.Property(e => e.AutoridadEntrega).HasColumnName("AutoridadEntrega")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatInstitucionesTraslado>(entity =>
        {
            entity.HasKey(e => e.IdInstitucionTraslado);

            entity.ToTable("catInstitucionesTraslado");
            entity.Property(e => e.IdInstitucionTraslado).HasColumnName("IdInstitucionTraslado");

            entity.Property(e => e.InstitucionTraslado).HasColumnName("InstitucionTraslado")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatOficinasRenta>(entity =>
        {
            entity.HasKey(e => e.IdOficinaRenta);

            entity.ToTable("catOficinasRenta");
            entity.Property(e => e.IdOficinaRenta).HasColumnName("IdOficinaRenta");

            entity.Property(e => e.NombreOficina).HasColumnName("NombreOficina")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatAgenciasMinisterio>(entity =>
        {
            entity.HasKey(e => e.IdAgenciaMinisterio);

            entity.ToTable("catAgenciasMinisterio");
            entity.Property(e => e.IdAgenciaMinisterio).HasColumnName("IdAgenciaMinisterio");

            entity.Property(e => e.NombreAgencia).HasColumnName("NombreAgencia")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatClasificacionAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdClasificacionAccidente);

            entity.ToTable("catClasificacionAccidentes");
            entity.Property(e => e.IdClasificacionAccidente).HasColumnName("IdClasificacionAccidente");

            entity.Property(e => e.NombreClasificacion).HasColumnName("NombreClasificacion")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatFactoresAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdFactorAccidente);

            entity.ToTable("catFactoresAccidentes");
            entity.Property(e => e.IdFactorAccidente).HasColumnName("IdFactorAccidente");

            entity.Property(e => e.FactorAccidente).HasColumnName("FactorAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatCausasAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdCausaAccidente);

            entity.ToTable("catCausasAccidentes");
            entity.Property(e => e.IdCausaAccidente).HasColumnName("IdCausaAccidente");

            entity.Property(e => e.CausaAccidente).HasColumnName("CausaAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatFactoresOpcionesAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdFactorOpcionAccidente);

            entity.ToTable("catFactoresOpcionesAccidentes");
            entity.Property(e => e.IdFactorOpcionAccidente).HasColumnName("IdFactorOpcionAccidente");

            entity.Property(e => e.FactorOpcionAccidente).HasColumnName("FactorOpcionAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdFactorAccidente).HasColumnName("IdFactorAccidente");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatHospitales>(entity =>
        {
            entity.HasKey(e => e.IdHospital);

            entity.ToTable("catHospitales");
            entity.Property(e => e.IdHospital).HasColumnName("IdHospital");

            entity.Property(e => e.NombreHospital).HasColumnName("NombreHospital")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdMunicipio).HasColumnName("IdMunicipio");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatDelegacionesOficinasTransporte>(entity =>
        {
            entity.HasKey(e => e.IdOficinaTransporte);

            entity.ToTable("catDelegacionesOficinasTransporte");
            entity.Property(e => e.IdOficinaTransporte).HasColumnName("IdOficinaTransporte");
            entity.Property(e => e.NombreOficina).HasColumnName("NombreOficina")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.JefeOficina).HasColumnName("JefeOficina")
               .HasMaxLength(100)
               .IsUnicode(false);
            entity.Property(e => e.IdMunicipio).HasColumnName("IdMunicipio");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        OnModelCreatingPartial(modelBuilder);    


        modelBuilder.Entity<Infracciones>(entity =>
            {
                entity.HasKey(e => e.IdInfraccion);

                entity.ToTable("infracciones");
                entity.Property(e => e.IdInfraccion).HasColumnName("IdInfraccion");
                entity.Property(e => e.FolioInfraccion).HasColumnName("folioInfraccion")
                 .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Placas).HasColumnName("placas")
                 .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.IdOficial).HasColumnName("idOficial");
                entity.Property(e => e.IdDependencia).HasColumnName("idDependencia");
                entity.Property(e => e.IdDelegacion).HasColumnName("idDelegacion");
                entity.Property(e=> e.Oficial).HasColumnName("oficial")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.Municipio).HasColumnName("municipio")
                   .HasMaxLength(150)
                   .IsUnicode(false);
                entity.Property(e => e.FechaInfraccion)
                .HasColumnType("datetime")
                .HasColumnName("fechaInfraccion");
                entity.Property(e => e.Carretera).HasColumnName("carretera")
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Tramo).HasColumnName("tramo")
                       .HasMaxLength(100)
                       .IsUnicode(false);
                 entity.Property(e => e.KmCarretera).HasColumnName("kmCarretera")
                   .HasMaxLength(100)
                   .IsUnicode(false);
                entity.Property(e => e.IdVehiculo).HasColumnName("idVehiculo");
                entity.Property(e => e.IdConductor).HasColumnName("IdConductor");
                entity.Property(e => e.Conductor).HasColumnName("conductor")
                      .HasMaxLength(150)
                      .IsUnicode(false);
                entity.Property(e => e.Propietario).HasColumnName("propietario")
                      .HasMaxLength(100)
                      .IsUnicode(false);
                entity.Property(e => e.IdAplicacion).HasColumnName("idAplicacion");
                entity.Property(e => e.InfraccionCortesia).HasColumnName("infraccionCortesia");
                entity.Property(e => e.IdGarantia).HasColumnName("idGarantia");
                entity.Property(e => e.EstatusProceso).HasColumnName("estatusProceso");
                entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
                entity.Property(e => e.FechaActualizacion)
                 .HasColumnType("datetime")
                 .HasColumnName("fechaActualizacion");

                entity.Property(e => e.Estatus).HasColumnName("Estatus");


            });
    }
      



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
