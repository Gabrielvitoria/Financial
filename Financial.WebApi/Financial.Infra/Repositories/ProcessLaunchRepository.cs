using Financial.Domain;
using Financial.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Financial.Infra.Repositories
{
    public class ProcessLaunchRepository : IProcessLaunchRepository
    {
        private DefaultContext _context;
        public ProcessLaunchRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Financiallaunch> CreateAsync(Financiallaunch launch)
        {
            await _context.Financiallaunch.AddAsync(launch);
            await _context.SaveChangesAsync();
            return launch;
        }

        public async Task<Financiallaunch> GetAsync(Guid launchId)
        {
            return await _context.Financiallaunch.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(launchId));
        }

        public async Task<Financiallaunch> GetByIdStatusOpenAsync(Guid launchId)
        {
            return await _context.Financiallaunch.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(launchId) && x.Status == launchStatusEnum.Open);
        }

        public async Task<Financiallaunch?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _context.Financiallaunch.AsNoTracking().FirstOrDefaultAsync(x => x.IdempotencyKey.Equals(new Guid(idempotencyKey)));
        }

        public async Task<Financiallaunch> UpdateAsync(Financiallaunch launch)
        {
            await _context.Financiallaunch.Where(x => x.Id.Equals(launch.Id))
                                          .ExecuteUpdateAsync(p => p.SetProperty(s => s.Status, launch.Status) 
                                                                    .SetProperty(s=> s.AlterDate, launch.AlterDate)
                                                                    .SetProperty(s=> s.Description, launch.Description)
                                                                    
                                                              );
            return launch;
        }
    }
}
