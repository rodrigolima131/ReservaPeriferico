using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories
{
    public class UsuarioEquipeRepository : Repository<UsuarioEquipe>, IUsuarioEquipeRepository
    {
        private new readonly ApplicationDbContext _context;

        public UsuarioEquipeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioEquipe>> GetByEquipeIdAsync(int equipeId)
        {
            return await _context.UsuarioEquipes
                .Include(ue => ue.Usuario)
                .Where(ue => ue.EquipeId == equipeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsuarioEquipe>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.UsuarioEquipes
                .Include(ue => ue.Equipe)
                .Where(ue => ue.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<UsuarioEquipe?> GetByUsuarioAndEquipeAsync(int usuarioId, int equipeId)
        {
            return await _context.UsuarioEquipes
                .Include(ue => ue.Usuario)
                .Include(ue => ue.Equipe)
                .FirstOrDefaultAsync(ue => ue.UsuarioId == usuarioId && ue.EquipeId == equipeId);
        }

        public async Task AddMembroAsync(int equipeId, int usuarioId, bool isAdministrador = false)
        {
            var usuarioEquipe = new UsuarioEquipe
            {
                EquipeId = equipeId,
                UsuarioId = usuarioId,
                IsAdministrador = isAdministrador,
                DataEntrada = DateTime.UtcNow
            };

            await _context.UsuarioEquipes.AddAsync(usuarioEquipe);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMembroAsync(int equipeId, int usuarioId)
        {
            var usuarioEquipe = await _context.UsuarioEquipes
                .FirstOrDefaultAsync(ue => ue.EquipeId == equipeId && ue.UsuarioId == usuarioId);

            if (usuarioEquipe != null)
            {
                _context.UsuarioEquipes.Remove(usuarioEquipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsuarioIsMembroAsync(int equipeId, int usuarioId)
        {
            return await _context.UsuarioEquipes
                .AnyAsync(ue => ue.EquipeId == equipeId && ue.UsuarioId == usuarioId);
        }

        public async Task<bool> UsuarioIsAdministradorAsync(int equipeId, int usuarioId)
        {
            return await _context.UsuarioEquipes
                .AnyAsync(ue => ue.EquipeId == equipeId && ue.UsuarioId == usuarioId && ue.IsAdministrador);
        }
    }
} 