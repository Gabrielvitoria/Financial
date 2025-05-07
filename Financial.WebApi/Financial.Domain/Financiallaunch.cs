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
            if (!createFinanciallaunchDto.IdempotencyKeyValid) return;

            Id = Guid.CreateVersion7();
            IdempotencyKey = new Guid(createFinanciallaunchDto.IdempotencyKey.ToUpper());
            LaunchType = createFinanciallaunchDto.LaunchType;
            PaymentMethod = createFinanciallaunchDto.PaymentMethod;
            Status = launchStatusEnum.Open;
            CoinType = createFinanciallaunchDto.CoinType;
            Value = createFinanciallaunchDto.Value;
            BankAccount = createFinanciallaunchDto.BankAccount;
            NameCustomerSupplier = createFinanciallaunchDto.NameCustomerSupplier;
            CostCenter = createFinanciallaunchDto.CostCenter;
            Description = createFinanciallaunchDto.Description;
            CreateDate = DateTime.UtcNow;
        }

        public Guid IdempotencyKey { get; private set; }
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
            this.Description += " [Cancel]: " + description;
            this.AlterDate = DateTime.Now;
        }

        public void PayOff()
        {
            this.Status = launchStatusEnum.PaidOff;
            this.Description += " [Paid off]: " + DateTime.UtcNow;
            this.AlterDate = DateTime.UtcNow;
        }    
    }
}
