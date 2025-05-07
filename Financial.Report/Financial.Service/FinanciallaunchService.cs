using Financial.Common;
using Financial.Domain.Dtos;
using Financial.Infra.Interfaces;
using Financial.Service.Interfaces;
using Financial.Service.Works;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Financial.Service
{
    public class FinanciallaunchService : IFinanciallaunchService
    {
        private readonly IFinanciallaunchRespository _financiallaunchRespository;
        private readonly ILogger<FinanciallaunchService> _logger;

        public FinanciallaunchService(IFinanciallaunchRespository financiallaunchRespository, ILogger<FinanciallaunchService> logger)
        {
            _financiallaunchRespository = financiallaunchRespository;
            _logger = logger;
        }

        public async Task ProcessesFinancialLauchAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
                if (financiallaunchEvent == null || financiallaunchEvent.Entity == null || financiallaunchEvent.Entity.Value == 0)
                {
                    _logger.LogWarning("FinanciallaunchService: FinanciallaunchEvent is null or has no value.");
                    return;
                }

                _logger.LogInformation($"FinanciallaunchService: FinanciallaunchEvent.Value: {financiallaunchEvent.Entity.Value}", financiallaunchEvent.Entity.Value);


                if (financiallaunchEvent.Entity.PaymentMethod == 1 || financiallaunchEvent.Entity.PaymentMethod == 2)
                {
                    await _financiallaunchRespository.SaveBalanceAsync(financiallaunchEvent.Entity.Value);
                }
                
                await _financiallaunchRespository.SaveLauchAsync(financiallaunchEvent.Entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task ProcessesPayFinancialLauchAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
                if (financiallaunchEvent == null || financiallaunchEvent.Entity == null || financiallaunchEvent.Entity.Value == 0)
                {
                    _logger.LogWarning("FinanciallaunchService: FinanciallaunchEvent is null or has no value.");
                    return;
                }

                _logger.LogInformation($"FinanciallaunchService: FinanciallaunchEvent.Value: {financiallaunchEvent.Entity.Value}", financiallaunchEvent.Entity.Value);
                              

                await _financiallaunchRespository.UpdateLauchAsync(financiallaunchEvent.Entity);

                await _financiallaunchRespository.SaveBalanceAsync(financiallaunchEvent.Entity.Value);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetDayBalanceAsync()
        {
            try
            {
                var saldo = await _financiallaunchRespository.GetBalanceAsync();

                return saldo.ToString("0.00", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FinanciallaunchDto>> GetDayLauchAsync()
        {
            try
            {
                return await _financiallaunchRespository.GetLauchAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
