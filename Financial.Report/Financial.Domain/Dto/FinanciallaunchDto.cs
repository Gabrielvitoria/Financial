using System.ComponentModel.DataAnnotations;

namespace Financial.Domain.Dtos
{
    public class FinanciallaunchDto : DtoBase
    {
        [Required]
        public string IdempotencyKey { get; set; }

        [Required]
        public launchTypeEnum LaunchType { get; set; }

        [Required]
        public launchPaymentMethodEnum PaymentMethod { get; set; }

        [Required]
        public launchStatusEnum Status { get; set; }

        [Required]
        public string CoinType { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        public string BankAccount { get; set; }

        [Required]
        public string NameCustomerSupplier { get; set; }

        [Required]
        public string CostCenter { get; set; }

        public string Description { get; set; }

    }
}
