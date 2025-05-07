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
        private readonly INotificationEvent _notificationEvent;

        public ProcessLaunchservice(IProcessLaunchRepository processLaunchRepository,
            INotificationEvent notificationEvent)
        {
            _processLaunchRepository = processLaunchRepository;
            _notificationEvent = notificationEvent;
        }


        public async Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            try
            {
                if (!createFinanciallaunchDto.IdempotencyKeyValid)
                {
                    throw new ApplicationException($"Error: Check if the data is correct. Some information that makes up the Idempotency is incorrect or does not match the idempotency");
                }

                var financialLaunchEntity = new Financiallaunch(createFinanciallaunchDto);

         
                var launchExist = await _processLaunchRepository.GetByIdempotencyKeyAsync(createFinanciallaunchDto.IdempotencyKey);

                if (launchExist != null)
                {
                    return launchExist.MapToDto();
                }

                var launch = await _processLaunchRepository.CreateAsync(financialLaunchEntity);

                await _notificationEvent.SendAsync(new FinanciallaunchEvent(launch));

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
        public Task<FinanciallaunchDto> ProcessCancelLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto)
        {
            throw new NotImplementedException();
        }

        public Task<FinanciallaunchDto> ProcessEditLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto)
        {
            throw new NotImplementedException();
        }
        public Task<FinanciallaunchDto> ProcessPayLaunchAsync(AlterFinanciallaunchDto alterFinanciallaunchDto)
        {
            throw new NotImplementedException();
        }
    }
}
