using Financial.Common;

namespace Financial.Service.Interfaces
{
    public interface IFinanciallaunchService
    {
        Task ProcessesFinancialLauchAsync(FinanciallaunchEvent financiallaunchEvent);
        Task<string> GetSaldoDiarioAsync();
    }
}
