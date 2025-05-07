using Financial.Domain.Dtos;
using Financial.Domain.Events;

namespace Financial.Infra.Interfaces
{
    public interface IFinanciallaunchRespository
    {
        Task<decimal> GetBalanceAsync();
        Task SaveBalanceAsync(decimal value);

        Task<List<FinanciallaunchDto>> GetLauchAsync();

        Task SaveLauchAsync(Financiallaunch value);

        Task UpdateLauchAsync(Financiallaunch value);
    }
}
