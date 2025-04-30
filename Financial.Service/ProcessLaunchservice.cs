using Financial.Domain;
using Financial.Domain.Dtos;
using Financial.Domain.Maps;
using Financial.Infra.Interfaces;
using Financial.Service.Interfaces;

namespace Financial.Service
{
    public class ProcessLaunchservice : IProcessLaunchservice
    {
        private readonly IProcessLaunchRepository _processLaunchRepository;

        public ProcessLaunchservice(IProcessLaunchRepository processLaunchRepository)
        {
            _processLaunchRepository = processLaunchRepository;
        }
        public async Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            try
            {
                var financialLaunchEntity = new Financiallaunch(createFinanciallaunchDto);

                if (!financialLaunchEntity.IdempotencyKeyValid)
                {
                    throw new ApplicationException($"Error: Check if the data is correct. Some information that makes up the Idempotency is incorrect or does not match the idempotency");
                }

                var launchExist = await _processLaunchRepository.GetByIdempotencyKeyAsync(financialLaunchEntity.IdempotencyKey);

                if (launchExist != null)
                {
                    return launchExist.MapToDto();
                }

                var launch = await _processLaunchRepository.CreateAsync(financialLaunchEntity);

                return launch.MapToDto();

            }
            catch (ApplicationException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
