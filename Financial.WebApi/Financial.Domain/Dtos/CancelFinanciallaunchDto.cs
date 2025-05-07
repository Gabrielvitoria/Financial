using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Financial.Domain.Dtos
{
    public class CancelFinanciallaunchDto 
    {
        [Required]
        public Guid Id { get; set; }

        public string Description { get; set; } = string.Empty;
       
    }
}
