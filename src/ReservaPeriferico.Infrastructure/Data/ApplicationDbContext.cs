using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Periferico> periferico { get; set; }
    public DbSet<Usuario> usuario { get; set; }
    public DbSet<Reserva> reserva { get; set; }
    public DbSet<Equipe> Equipes { get; set; }
    public DbSet<UsuarioEquipe> UsuarioEquipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Periferico
        modelBuilder.Entity<Periferico>(entity =>
        {
            entity.ToTable("periferico");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(500);
            entity.Property(e => e.Tipo).HasColumnName("tipo").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Marca).HasColumnName("marca").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Modelo).HasColumnName("modelo").HasMaxLength(50);
            entity.Property(e => e.NumeroSerie).HasColumnName("numero_serie").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Ativo).HasColumnName("ativo");
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(e => e.DataAtualizacao).HasColumnName("data_atualizacao");
            
            // Configurar DateTime para UTC
            entity.Property(e => e.DataCadastro).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.DataAtualizacao).HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            
            // Índice único para número de série
            entity.HasIndex(e => e.NumeroSerie).IsUnique();
        });

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Matricula).HasColumnName("matricula").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Departamento).HasColumnName("departamento").HasMaxLength(50);
            entity.Property(e => e.Cargo).HasColumnName("cargo").HasMaxLength(50);
            entity.Property(e => e.Ativo).HasColumnName("ativo");
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(e => e.DataAtualizacao).HasColumnName("data_atualizacao");
            
            // Configurar DateTime para UTC
            entity.Property(e => e.DataCadastro).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.DataAtualizacao).HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            
            // Índices únicos para email e matrícula
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Matricula).IsUnique();

            // Seed de usuário padrão para desenvolvimento
            entity.HasData(new Usuario
            {
                Id = 1,
                Nome = "Usuário Teste",
                Email = "teste@empresa.com",
                Matricula = "TESTE001",
                Departamento = "TI",
                Cargo = "Desenvolvedor",
                Ativo = true,
                DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        });

        // Configuração da entidade Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reserva");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.PerifericoId).HasColumnName("periferico_id");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");
            entity.Property(e => e.Observacoes).HasColumnName("observacoes").HasMaxLength(500);
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(e => e.DataAtualizacao).HasColumnName("data_atualizacao");
            entity.Property(e => e.DataDevolucao).HasColumnName("data_devolucao");
            
            // Configurar DateTime para UTC
            entity.Property(e => e.DataCadastro).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.DataAtualizacao).HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            entity.Property(e => e.DataInicio).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.DataFim).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.DataDevolucao).HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            
            // Relacionamentos
            entity.HasOne(e => e.Usuario)
                  .WithMany(e => e.Reservas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Periferico)
                  .WithMany(e => e.Reservas)
                  .HasForeignKey(e => e.PerifericoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade Equipe
        modelBuilder.Entity<Equipe>(entity =>
        {
            entity.ToTable("equipe");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(500);
            entity.Property(e => e.UsuarioAdministradorId).HasColumnName("usuario_administrador_id").IsRequired();
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(e => e.DataAtualizacao).HasColumnName("data_atualizacao");
            entity.HasIndex(e => e.Nome).IsUnique();
        });

        // Configuração da entidade UsuarioEquipe
        modelBuilder.Entity<UsuarioEquipe>(entity =>
        {
            entity.ToTable("usuario_equipe");
            entity.HasKey(ue => new { ue.UsuarioId, ue.EquipeId });
            entity.Property(ue => ue.UsuarioId).HasColumnName("usuario_id");
            entity.Property(ue => ue.EquipeId).HasColumnName("equipe_id");
            entity.Property(ue => ue.IsAdministrador).HasColumnName("is_administrador");
            entity.Property(ue => ue.DataEntrada).HasColumnName("data_entrada");
            entity.HasOne(ue => ue.Usuario)
                  .WithMany()
                  .HasForeignKey(ue => ue.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(ue => ue.Equipe)
                  .WithMany(e => e.Membros)
                  .HasForeignKey(ue => ue.EquipeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Periferico || e.Entity is Usuario || e.Entity is Reserva)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            if (entry.State == EntityState.Added)
            {
                if (entity is Periferico periferico)
                {
                    periferico.DataCadastro = DateTime.UtcNow;
                }
                else if (entity is Usuario usuario)
                {
                    usuario.DataCadastro = DateTime.UtcNow;
                }
                else if (entity is Reserva reserva)
                {
                    reserva.DataCadastro = DateTime.UtcNow;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (entity is Periferico periferico)
                {
                    periferico.DataAtualizacao = DateTime.UtcNow;
                }
                else if (entity is Usuario usuario)
                {
                    usuario.DataAtualizacao = DateTime.UtcNow;
                }
                else if (entity is Reserva reserva)
                {
                    reserva.DataAtualizacao = DateTime.UtcNow;
                }
            }
        }
    }
} 