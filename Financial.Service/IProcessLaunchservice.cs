using Financial.Domain.Dtos;

namespace Financial.Service
{
    public interface IProcessLaunchservice
    {
        Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto);
    }
}
