using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories
{
    public class EquipeRepository : Repository<Equipe>, IEquipeRepository
    {
        private readonly ApplicationDbContext _context;

        public EquipeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Equipe?> GetByIdAsync(int id)
        {
            return await _context.Equipes
                .Include(e => e.Membros)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public override async Task<IEnumerable<Equipe>> GetAllAsync()
        {
            return await _context.Equipes
                .Include(e => e.Membros)
                .ToListAsync();
        }

        public async Task<Equipe?> GetByNomeAsync(string nome)
        {
            return await _context.Equipes
                .Include(e => e.Membros)
                .FirstOrDefaultAsync(e => e.Nome == nome);
        }
    }
} 