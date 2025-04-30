using Financial.Domain.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain
{
    public class Financiallaunch : Base
    {
        public Financiallaunch()
        {
            
        }
        public Financiallaunch(CreateFinanciallaunchDto createFinanciallaunchDto)
        {
            Id = Guid.CreateVersion7();
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
            CreateDate = DateTime.Now;
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

        public void Cancel(string? description = null)
        {
            this.Status = launchStatusEnum.Canceled;
            this.Description += description ??  " Cancel: " + description;
            this.AlterDate = DateTime.Now;
        }

        public void PayOff()
        {
            this.Status = launchStatusEnum.PaidOff;
            this.Description += " Paid off." ;
            this.AlterDate = DateTime.Now;
        }

        [NotMapped]
        public bool IdempotencyKeyValid => this.IdempotencyKey.Equals(this.GetIdempotencyKey);

        [NotMapped]
        public string GetIdempotencyKey => $"{nameof(Financiallaunch)}_{this.LaunchType}_{this.PaymentMethod}_{this.Value.ToString()}_{this.BankAccount}_{this.NameCustomerSupplier}_UNIQUESUFFIX";

    }
}
