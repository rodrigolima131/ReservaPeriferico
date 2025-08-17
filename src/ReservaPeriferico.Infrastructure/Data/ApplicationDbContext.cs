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
            entity.Property(e => e.EquipeId).HasColumnName("equipe_id");
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
            
            // Relacionamento com Equipe
            entity.HasOne(e => e.Equipe)
                  .WithMany(e => e.Perifericos)
                  .HasForeignKey(e => e.EquipeId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Seed de periféricos padrão para desenvolvimento
            entity.HasData(
                new Periferico
                {
                    Id = 1,
                    Nome = "Monitor Dell 24\"",
                    Descricao = "Monitor LED de 24 polegadas para desenvolvimento",
                    Tipo = "Monitor",
                    Marca = "Dell",
                    Modelo = "P2419H",
                    NumeroSerie = "DELL001",
                    Ativo = true,
                    EquipeId = 1,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Periferico
                {
                    Id = 2,
                    Nome = "Teclado Mecânico Logitech",
                    Descricao = "Teclado mecânico com switches Cherry MX Brown",
                    Tipo = "Teclado",
                    Marca = "Logitech",
                    Modelo = "G Pro X",
                    NumeroSerie = "LOG001",
                    Ativo = true,
                    EquipeId = 1,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Periferico
                {
                    Id = 3,
                    Nome = "Mouse Gamer Razer",
                    Descricao = "Mouse óptico com DPI ajustável",
                    Tipo = "Mouse",
                    Marca = "Razer",
                    Modelo = "DeathAdder V3",
                    NumeroSerie = "RAZ001",
                    Ativo = true,
                    EquipeId = 1,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Periferico
                {
                    Id = 4,
                    Nome = "Headset Sony WH-1000XM4",
                    Descricao = "Fones de ouvido com cancelamento de ruído",
                    Tipo = "Headset",
                    Marca = "Sony",
                    Modelo = "WH-1000XM4",
                    NumeroSerie = "SON001",
                    Ativo = true,
                    EquipeId = 1,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Periferico
                {
                    Id = 5,
                    Nome = "Webcam Logitech C920",
                    Descricao = "Webcam HD para reuniões e videoconferências",
                    Tipo = "Webcam",
                    Marca = "Logitech",
                    Modelo = "C920",
                    NumeroSerie = "LOG002",
                    Ativo = true,
                    EquipeId = 1,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
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
            entity.Property(e => e.EquipeId).HasColumnName("equipe_id");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");
            entity.Property(e => e.Observacoes).HasColumnName("observacoes").HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UsuarioAprovadorId).HasColumnName("usuario_aprovador_id");
            entity.Property(e => e.DataAprovacao).HasColumnName("data_aprovacao");
            entity.Property(e => e.MotivoRejeicao).HasColumnName("motivo_rejeicao").HasMaxLength(500);
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
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            entity.Property(e => e.DataAprovacao).HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
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
                  
            entity.HasOne(e => e.Equipe)
                  .WithMany(e => e.Reservas)
                  .HasForeignKey(e => e.EquipeId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.UsuarioAprovador)
                  .WithMany(e => e.ReservasAprovadas)
                  .HasForeignKey(e => e.UsuarioAprovadorId)
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
            
            // Relacionamento com UsuarioAdministrador
            entity.HasOne(e => e.UsuarioAdministrador)
                  .WithMany(e => e.EquipesAdministradas)
                  .HasForeignKey(e => e.UsuarioAdministradorId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Seed de equipe padrão para desenvolvimento
            entity.HasData(new Equipe
            {
                Id = 1,
                Nome = "Equipe de Desenvolvimento",
                Descricao = "Equipe responsável pelo desenvolvimento de software",
                UsuarioAdministradorId = 1,
                DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
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
            
            // Configurar DateTime para UTC
            entity.Property(ue => ue.DataEntrada).HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            
            entity.HasOne(ue => ue.Usuario)
                  .WithMany(e => e.Equipes)
                  .HasForeignKey(ue => ue.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(ue => ue.Equipe)
                  .WithMany(e => e.Membros)
                  .HasForeignKey(ue => ue.EquipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Seed de usuário na equipe padrão
            entity.HasData(new UsuarioEquipe
            {
                UsuarioId = 1,
                EquipeId = 1,
                IsAdministrador = true,
                DataEntrada = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
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
            .Where(e => e.Entity is Periferico || e.Entity is Usuario || e.Entity is Reserva || e.Entity is Equipe)
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
                else if (entity is Equipe equipe)
                {
                    equipe.DataCadastro = DateTime.UtcNow;
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
                else if (entity is Equipe equipe)
                {
                    equipe.DataAtualizacao = DateTime.UtcNow;
                }
            }
        }
    }
} 