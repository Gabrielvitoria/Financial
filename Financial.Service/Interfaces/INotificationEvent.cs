using Financial.Domain.Dtos;
using System.Diagnostics.Tracing;

namespace Financial.Service.Interfaces
{
    public interface INotificationEvent
    {
        Task SendAsync(FinanciallaunchEvent financiallaunchEvent);
    }
}
