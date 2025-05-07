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

                if (financialLaunchEntity.PaymentMethod == launchPaymentMethodEnum.Cash)
                {
                    financialLaunchEntity.PayOff();
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

        public async Task<FinanciallaunchDto> ProcessCancelLaunchAsync(CancelFinanciallaunchDto cancelFinanciallaunchDto)
        {
            if (cancelFinanciallaunchDto.Id == Guid.Empty)
            {
                throw new ApplicationException($"Error: Check if the data is correct. Some information that makes up the Id is incorrect or does not match the ID");
            }

            var launchExist = await _processLaunchRepository.GetByIdStatusOpenAsync(cancelFinanciallaunchDto.Id);

            if (launchExist == null)
            {
                throw new ApplicationException($"Info: The release cannot be canceled. Status other than \"Open\"");
            }

            launchExist.Cancel(cancelFinanciallaunchDto.Description);

            var launch = await _processLaunchRepository.UpdateAsync(launchExist);

            await _notificationEvent.SendCancelAsync(new FinanciallaunchEvent(launch));


            return launch.MapToDto();
        }

        public async Task<FinanciallaunchDto> ProcessPayLaunchAsync(PayFinanciallaunchDto payFinanciallaunchDto)
        {
            if (payFinanciallaunchDto.Id == Guid.Empty)
            {
                throw new ApplicationException($"Error: Check if the data is correct. Some information that makes up the Id is incorrect or does not match the ID");
            }

            var launchExist = await _processLaunchRepository.GetByIdStatusOpenAsync(payFinanciallaunchDto.Id);

            if (launchExist == null)
            {
                throw new ApplicationException($"Info: The release cannot be canceled. Status other than \"Open\"");
            }

            launchExist.PayOff();

            var launch = await _processLaunchRepository.UpdateAsync(launchExist);

            await _notificationEvent.SendPaidAsync(new FinanciallaunchEvent(launch));

            return launch.MapToDto();
        }


    }
}
