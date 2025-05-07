using Financial.Domain.Dtos;

namespace Financial.Service.Interfaces
{
    public interface IProcessLaunchservice
    {
        Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto);
           
        Task<FinanciallaunchDto> ProcessPayLaunchAsync(PayFinanciallaunchDto alterFinanciallaunchDto);

        Task<FinanciallaunchDto> ProcessCancelLaunchAsync(CancelFinanciallaunchDto alterFinanciallaunchDto);

    }
}
