using Financial.Common;
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
                if(financiallaunchEvent == null || financiallaunchEvent.Entity == null || financiallaunchEvent.Entity.Value == 0)
                {
                    _logger.LogWarning("FinanciallaunchService: FinanciallaunchEvent is null or has no value.");
                    return;
                }   

                _logger.LogInformation($"FinanciallaunchService: FinanciallaunchEvent.Value: {financiallaunchEvent.Entity.Value}", financiallaunchEvent.Entity.Value);

                await _financiallaunchRespository.SaveAsync(financiallaunchEvent.Entity.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetSaldoDiarioAsync()
        {
            try
            {
                var saldo = await _financiallaunchRespository.GetAsync();

                return saldo.ToString("0.00", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FormatSaldo(long saldoFromRedis)
        {
            if (saldoFromRedis == 0)
            {
                return "0.00";
            }

            decimal saldoDecimal = saldoFromRedis / 1000m;
            return saldoDecimal.ToString("0.00", CultureInfo.InvariantCulture);

        }



    }
}
