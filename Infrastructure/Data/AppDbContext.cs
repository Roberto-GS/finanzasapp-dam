using FinanzasApp.Core.Enums;
using FinanzasApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzasApp.Infrastructure.Data;

/// <summary>
/// Es el contexto para la base de datos de la aplicación. Hereda de DbContext y define las entidades
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ENTIDADES
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<Objetivo> Objetivos => Set<Objetivo>();
    public DbSet<TipoObjetivo> TiposObjetivo => Set<TipoObjetivo>();
    public DbSet<Grupo> Grupos => Set<Grupo>();
    public DbSet<UsuarioGrupo> UsuariosGrupos => Set<UsuarioGrupo>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<SolicitudGrupo> SolicitudesGrupo => Set<SolicitudGrupo>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    // Conversores entre enum y string para MySQL

    /// <summary>
    /// Convierte un PeriodoObjetivo a String para la BD.
    /// </summary>
    private static string ConvertirPeriodoATexto(PeriodoObjetivo periodo)
        => periodo.ToString().ToLowerInvariant();

    /// <summary>
    /// Convierte un String de la BD a su valor enum PeriodoObjetivo.
    /// </summary>
    private static PeriodoObjetivo ConvertirTextoPeriodo(string texto)
        => Enum.Parse<PeriodoObjetivo>(texto, true);

    /// <summary>
    /// Convierte un TipoMovimiento a String para la BD.
    /// </summary>
    private static string ConvertirTipoMovimientoATexto(TipoMovimiento tipo)
        => tipo == TipoMovimiento.Ingreso ? "ingreso" : "gasto";

    /// <summary>
    /// Convierte un texto de la BD a su valor enum TipoMovimiento.
    /// </summary>
    private static TipoMovimiento ConvertirTextoTipoMovimiento(string texto)
        => texto == "ingreso" ? TipoMovimiento.Ingreso : TipoMovimiento.Gasto;

    /// <summary>
    /// Configuramos el mapeo de todas las entidades con sus tablas, columnas, relaciones e índices en la base de datos.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("Usuarios");
            entidad.HasKey(u => u.Id);

            entidad.Property(u => u.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            entidad.Property(u => u.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(120)
                   .IsRequired();

            entidad.Property(u => u.Email)
                   .HasColumnName("email")
                   .HasMaxLength(150)
                   .IsRequired();

            entidad.HasIndex(u => u.Email).IsUnique();

            entidad.Property(u => u.PasswordHash)
                   .HasColumnName("password_hash")
                   .HasMaxLength(255)
                   .IsRequired();

            entidad.Property(u => u.FechaCreacion)
                   .HasColumnName("fecha_creacion")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Ignoramos las colecciones de navegación, para evitar que EF Core genere columnas fantasma
            entidad.Ignore(u => u.Movimientos);
            entidad.Ignore(u => u.Categorias);
            entidad.Ignore(u => u.Objetivos);
        });

        // Categoria
        modelBuilder.Entity<Categoria>(entidad =>
        {
            entidad.ToTable("Categorias");
            entidad.HasKey(c => c.Id);

            entidad.Property(c => c.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            entidad.Property(c => c.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(50)
                   .IsRequired();

            entidad.Property(c => c.Color)
                   .HasColumnName("color")
                   .HasMaxLength(10);

            entidad.Property(c => c.UsuarioId)
                   .HasColumnName("usuario_id");

            entidad.HasOne(c => c.Usuario)
                   .WithMany()
                   .HasForeignKey(c => c.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Ignoramos colecciones de navegación
            entidad.Ignore(c => c.Movimientos);
            entidad.Ignore(c => c.Objetivos);
        });

        // Movimiento
        modelBuilder.Entity<Movimiento>(entidad =>
        {
            entidad.ToTable("Movimientos");
            entidad.HasKey(m => m.Id);

            entidad.Property(m => m.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            entidad.Property(m => m.UsuarioId)
                   .HasColumnName("usuario_id")
                   .IsRequired();

            // Convertimos el enum TipoMovimiento a String para MySQL
            entidad.Property(m => m.Tipo)
                   .HasColumnName("tipo")
                   .HasConversion(
                       v => ConvertirTipoMovimientoATexto(v),
                       v => ConvertirTextoTipoMovimiento(v))
                   .IsRequired();

            entidad.Property(m => m.Cantidad)
                   .HasColumnName("cantidad")
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            entidad.Property(m => m.Fecha)
                   .HasColumnName("fecha")
                   .IsRequired();

            entidad.Property(m => m.CategoriaId)
                   .HasColumnName("categoria_id");

            entidad.Property(m => m.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(100)
                   .IsRequired();

            entidad.Property(m => m.Descripcion)
                   .HasColumnName("descripcion");

            entidad.Property(m => m.Etiqueta)
                   .HasColumnName("etiqueta")
                   .HasMaxLength(50);

            entidad.HasOne(m => m.Usuario)
                   .WithMany()
                   .HasForeignKey(m => m.UsuarioId)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(m => m.Categoria)
                   .WithMany()
                   .HasForeignKey(m => m.CategoriaId)
                   .HasPrincipalKey(c => c.Id)
                   .OnDelete(DeleteBehavior.SetNull);

            // Índices para mejorar el rendimiento de las consultas más frecuentes
            entidad.HasIndex(m => m.UsuarioId).HasDatabaseName("idx_mov_usuario");
            entidad.HasIndex(m => m.Fecha).HasDatabaseName("idx_mov_fecha");
            entidad.HasIndex(m => m.CategoriaId).HasDatabaseName("idx_mov_categoria");
        });

        // TipoObjetivo
        modelBuilder.Entity<TipoObjetivo>(entidad =>
        {
            entidad.ToTable("Tipos_Objetivo");
            entidad.HasKey(t => t.Id);

            entidad.Property(t => t.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            entidad.Property(t => t.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(50)
                   .IsRequired();

            entidad.HasIndex(t => t.Nombre).IsUnique();
            entidad.Ignore(t => t.Objetivos);
        });

        // Objetivo
        modelBuilder.Entity<Objetivo>(entidad =>
        {
            entidad.ToTable("Objetivos");
            entidad.HasKey(o => o.Id);

            entidad.Property(o => o.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            entidad.Property(o => o.UsuarioId)
                   .HasColumnName("usuario_id")
                   .IsRequired();

            entidad.Property(o => o.TipoId)
                   .HasColumnName("tipo_id")
                   .IsRequired();

            entidad.Property(o => o.CategoriaId)
                   .HasColumnName("categoria_id");

            entidad.Property(o => o.CantidadObjetivo)
                   .HasColumnName("cantidad_objetivo")
                   .HasColumnType("decimal(10,2)");

            // Convertimos el enum PeriodoObjetivo a String para MySQL
            entidad.Property(o => o.Periodo)
                   .HasColumnName("periodo")
                   .HasConversion(
                       v => ConvertirPeriodoATexto(v),
                       v => ConvertirTextoPeriodo(v))
                   .IsRequired();

            entidad.Property(o => o.Descripcion)
                   .HasColumnName("descripcion")
                   .HasMaxLength(255);

            entidad.Property(o => o.FechaInicio)
                   .HasColumnName("fecha_inicio");

            entidad.Property(o => o.FechaFin)
                   .HasColumnName("fecha_fin");

            entidad.Property(o => o.Activo)
                   .HasColumnName("activo")
                   .HasDefaultValue(true);

            // EstaVencido es una propiedad que calcula el programa, no se guarda en la BD
            entidad.Ignore(o => o.EstaVencido);

            entidad.HasOne(o => o.Usuario)
                   .WithMany()
                   .HasForeignKey(o => o.UsuarioId)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(o => o.TipoObjetivo)
                   .WithMany()
                   .HasForeignKey(o => o.TipoId)
                   .HasPrincipalKey(t => t.Id)
                   .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(o => o.Categoria)
                   .WithMany()
                   .HasForeignKey(o => o.CategoriaId)
                   .HasPrincipalKey(c => c.Id)
                   .OnDelete(DeleteBehavior.SetNull);
        });

        // Rol
        modelBuilder.Entity<Rol>(entidad =>
        {
            entidad.ToTable("Roles");
            entidad.HasKey(r => r.Id);
            entidad.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entidad.Property(r => r.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entidad.HasIndex(r => r.Nombre).IsUnique();
        });

        // Grupo
        modelBuilder.Entity<Grupo>(entidad =>
        {
            entidad.ToTable("Grupos");
            entidad.HasKey(g => g.Id);
            entidad.Property(g => g.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entidad.Property(g => g.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entidad.Property(g => g.CreadorId).HasColumnName("creador_id").IsRequired();
            entidad.Property(g => g.FechaCreacion)
                   .HasColumnName("fecha_creacion")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entidad.HasOne(g => g.Creador)
                   .WithMany()
                   .HasForeignKey(g => g.CreadorId)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.Ignore(g => g.Miembros);
        });

        // UsuarioGrupo
        modelBuilder.Entity<UsuarioGrupo>(entidad =>
        {
            entidad.ToTable("Usuarios_Grupos");
            entidad.HasKey(ug => new { ug.UsuarioId, ug.GrupoId });
            entidad.Property(ug => ug.UsuarioId).HasColumnName("usuario_id");
            entidad.Property(ug => ug.GrupoId).HasColumnName("grupo_id");
            entidad.Property(ug => ug.RolId).HasColumnName("rol_id");
            entidad.Property(ug => ug.FechaUnion)
                   .HasColumnName("fecha_union")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entidad.HasOne(ug => ug.Usuario)
                   .WithMany()
                   .HasForeignKey(ug => ug.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(ug => ug.Grupo)
                   .WithMany()
                   .HasForeignKey(ug => ug.GrupoId)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(ug => ug.Rol)
                   .WithMany()
                   .HasForeignKey(ug => ug.RolId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // SolicitudGrupo
        modelBuilder.Entity<SolicitudGrupo>(entidad =>
        {
            entidad.ToTable("Solicitudes_Grupo");
            entidad.HasKey(s => s.Id);
            entidad.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entidad.Property(s => s.GrupoId).HasColumnName("grupo_id");
            entidad.Property(s => s.SolicitanteId).HasColumnName("solicitante_id");
            entidad.Property(s => s.InvitadoId).HasColumnName("invitado_id");
            entidad.Property(s => s.Estado).HasColumnName("estado").HasMaxLength(20);
            entidad.Property(s => s.FechaSolicitud)
                   .HasColumnName("fecha_solicitud")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entidad.HasOne(s => s.Grupo)
                   .WithMany()
                   .HasForeignKey(s => s.GrupoId)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(s => s.Solicitante)
                   .WithMany()
                   .HasForeignKey(s => s.SolicitanteId)
                   .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(s => s.Invitado)
                   .WithMany()
                   .HasForeignKey(s => s.InvitadoId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // Notificacion
        modelBuilder.Entity<Notificacion>(entidad =>
        {
            entidad.ToTable("Notificaciones");
            entidad.HasKey(n => n.Id);
            entidad.Property(n => n.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entidad.Property(n => n.UsuarioId).HasColumnName("usuario_id").IsRequired();
            entidad.Property(n => n.ObjetivoId).HasColumnName("objetivo_id");
            entidad.Property(n => n.Titulo).HasColumnName("titulo").HasMaxLength(150).IsRequired();
            entidad.Property(n => n.Mensaje).HasColumnName("mensaje").HasMaxLength(500).IsRequired();
            entidad.Property(n => n.Tipo).HasColumnName("tipo").HasMaxLength(30).IsRequired();
            entidad.Property(n => n.Icono).HasColumnName("icono").HasMaxLength(10).IsRequired();
            entidad.Property(n => n.Leida).HasColumnName("leida").HasDefaultValue(false);
            entidad.Property(n => n.FechaCreacion)
                   .HasColumnName("fecha_creacion")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entidad.HasOne(n => n.Usuario)
                   .WithMany()
                   .HasForeignKey(n => n.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            entidad.HasOne(n => n.Objetivo)
                   .WithMany()
                   .HasForeignKey(n => n.ObjetivoId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}