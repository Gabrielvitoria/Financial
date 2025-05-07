using Financial.Domain;

namespace Financial.Infra.Interfaces
{
    public interface IProcessLaunchRepository
    {
        Task<Financiallaunch> CreateAsync(Financiallaunch launch);   
        Task<Financiallaunch> UpdateAsync(Financiallaunch launch);   
        Task<Financiallaunch> GetAsync(Guid launchId);
        Task<Financiallaunch> GetByIdStatusOpenAsync(Guid launchId);
        Task<Financiallaunch> GetByIdempotencyKeyAsync(string idempotencyKey);
    }
}
