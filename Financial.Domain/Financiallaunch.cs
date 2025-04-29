using Financial.Domain.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain
{
    public class Financiallaunch
    {
        public Financiallaunch(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            IdempotencyKey = createFinanciallaunchDto.IdempotencyKey;
            LaunchType = createFinanciallaunchDto.LaunchType;
            PaymentMethod = createFinanciallaunchDto.PaymentMethod;
            Status = launchStatusEnum.Open;
            CoinType = createFinanciallaunchDto.CoinType;
            Value = createFinanciallaunchDto.Value;
            BankAccount = createFinanciallaunchDto.BankAccount;
            NameCustomerSupplier = createFinanciallaunchDto.NameCustomerSupplier;
            CostCenter = createFinanciallaunchDto.CostCenter;
            Description = createFinanciallaunchDto.Description;
        }

        public string IdempotencyKey { get; private set; }
        public launchTypeEnum LaunchType { get; private set; }
        public launchPaymentMethodEnum PaymentMethod { get; private set; }
        public launchStatusEnum Status { get; private set; }
        public string CoinType { get; private set; }
        public decimal Value { get; private set; }
        public string BankAccount { get; private set; }
        public string NameCustomerSupplier { get; private set; }
        public string CostCenter { get; private set; }
        public string Description { get; private set; }


        [NotMapped]
        public bool IdempotencyKeyValid => this.IdempotencyKey.Equals(this.GetIdempotencyKey);

        [NotMapped]
        public string GetIdempotencyKey => $"{nameof(Financiallaunch)}_{this.LaunchType}_{this.PaymentMethod}_{this.Value}_{this.BankAccount}_{this.NameCustomerSupplier}_UNIQUESUFFIX";
    }
}
