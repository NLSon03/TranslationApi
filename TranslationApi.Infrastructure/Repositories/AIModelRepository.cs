using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;

namespace TranslationApi.Infrastructure.Repositories
{
    public class AIModelRepository : Repository<AIModel>, IAIModelRepository
    {
        public AIModelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AIModel>> GetActiveModelsAsync()
        {
            return await _dbSet.Where(m => m.IsActive).ToListAsync();
        }

        public async Task<AIModel?> GetByNameAndVersionAsync(string name, string version)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name == name && m.Version == version);
        }

        public async Task DeactivateModelAsync(Guid id)
        {
            var model = await _dbSet.FindAsync(id);
            if (model != null)
            {
                model.IsActive = false;
                await UpdateAsync(model);
            }
        }

        public async Task<bool> ExistsAsync(string name, string version)
        {
            return await _dbSet.AnyAsync(m => m.Name == name && m.Version == version);
        }
    }
} 