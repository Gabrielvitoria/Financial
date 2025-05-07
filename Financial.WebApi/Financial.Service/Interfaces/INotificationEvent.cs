using Financial.Domain.Dtos;
using System.Diagnostics.Tracing;

namespace Financial.Service.Interfaces
{
    public interface INotificationEvent
    {
        Task<bool> SendAsync(FinanciallaunchEvent financiallaunchEvent);
        Task<bool> SendCancelAsync(FinanciallaunchEvent financiallaunchEvent);
        Task<bool> SendPaidAsync(FinanciallaunchEvent financiallaunchEvent);
    }
}
