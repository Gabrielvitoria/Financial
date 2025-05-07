using Financial.Common;
using Financial.Domain.Dtos;

namespace Financial.Service.Interfaces
{
    public interface IFinanciallaunchService
    {
        Task ProcessesFinancialLauchAsync(FinanciallaunchEvent financiallaunchEvent);
        Task ProcessesPayFinancialLauchAsync(FinanciallaunchEvent financiallaunchEvent);
        Task<string> GetDayBalanceAsync();

        Task<List<FinanciallaunchDto>> GetDayLauchAsync();
    }
}
