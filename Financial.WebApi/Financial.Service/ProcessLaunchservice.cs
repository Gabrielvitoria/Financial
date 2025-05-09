using Financial.Domain;
using Financial.Domain.Dtos;
using Financial.Domain.Maps;
using Financial.Infra.Interfaces;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Financial.Service
{
    public class ProcessLaunchservice : IProcessLaunchservice
    {
        private readonly IProcessLaunchRepository _processLaunchRepository;
        private readonly INotificationEvent _notificationEvent;
        private readonly ILogger<ProcessLaunchservice> _logger;


        public ProcessLaunchservice(IProcessLaunchRepository processLaunchRepository,
            INotificationEvent notificationEvent, ILogger<ProcessLaunchservice> logger)
        {
            _processLaunchRepository = processLaunchRepository;
            _notificationEvent = notificationEvent;
            _logger = logger;
        }


        public async Task<FinanciallaunchDto> ProcessNewLaunchAsync(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            try
            {
                if (!createFinanciallaunchDto.IdempotencyKeyValid)
                {
                    var msg = $"Error: Check if the data is correct. Some information that makes up the Idempotency is incorrect or does not match the idempotency";
                    _logger.LogError(msg);
                    throw new ApplicationException(msg);
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

                _logger.LogInformation($"Launch created: {launch.Id}");

                return launch.MapToDto();

            }
            catch (ApplicationException aex)
            {
                _logger.LogError($"Erro: ", aex.Message);
                throw aex;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: ", ex.Message);
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
                var msg = $"Error: Check if the data is correct. Some information that makes up the Id is incorrect or does not match the ID";
                _logger.LogError(msg);
                throw new ApplicationException(msg);
            }

            var launchExist = await _processLaunchRepository.GetByIdStatusOpenAsync(payFinanciallaunchDto.Id);

            if (launchExist == null)
            {
                var msg = $"Info: The release cannot be canceled. Status other than \"Open\"";
                _logger.LogError(msg);
                throw new ApplicationException(msg);
            }

            launchExist.PayOff();

            var launch = await _processLaunchRepository.UpdateAsync(launchExist);

            await _notificationEvent.SendPaidAsync(new FinanciallaunchEvent(launch));

            _logger.LogInformation($"Launch paid: {launch.Id}");

            return launch.MapToDto();
        }


    }
}
