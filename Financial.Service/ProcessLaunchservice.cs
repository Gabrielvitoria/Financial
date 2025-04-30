using Financial.Domain.Dtos;
using Financial.Service.Interfaces;

namespace Financial.Service
{
    public class ProcessLaunchservice : IProcessLaunchservice
    {
        public Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            throw new NotImplementedException();
        }
    }
}
