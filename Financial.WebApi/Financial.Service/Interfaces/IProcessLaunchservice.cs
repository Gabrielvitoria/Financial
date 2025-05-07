using Financial.Domain.Dtos;

namespace Financial.Service.Interfaces
{
    public interface IProcessLaunchservice
    {
        Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto);

        Task<FinanciallaunchDto> ProcessEditLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto);

        Task<FinanciallaunchDto> ProcessPayLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto);

        Task<FinanciallaunchDto> ProcessCancelLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto);

    }
}
