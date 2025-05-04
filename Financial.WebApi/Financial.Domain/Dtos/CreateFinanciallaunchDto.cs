using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financial.Domain.Dtos
{
    public class CreateFinanciallaunchDto
    {
        [Required]
        public string IdempotencyKey { get; set; }

        [Required]
        public launchTypeEnum LaunchType { get; set; }

        [Required]
        public launchPaymentMethodEnum PaymentMethod { get; set; }


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


        [NotMapped]
        public bool IdempotencyKeyValid
        {
            get
            {

                Guid parsedGuid;

                if (Guid.TryParse(IdempotencyKey, out parsedGuid))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
