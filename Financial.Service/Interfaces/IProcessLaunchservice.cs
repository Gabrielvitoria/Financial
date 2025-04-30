using Financial.Domain.Dtos;

namespace Financial.Service.Interfaces
{
    public interface IProcessLaunchservice
    {
        Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto);
    }
}
