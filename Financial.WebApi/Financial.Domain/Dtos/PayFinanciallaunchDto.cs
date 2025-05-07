using System.ComponentModel.DataAnnotations;

namespace Financial.Domain.Dtos
{
    public class PayFinanciallaunchDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
