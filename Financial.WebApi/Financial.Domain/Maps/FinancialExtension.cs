using Financial.Domain.Dtos;

namespace Financial.Domain.Maps
{
    public static class FinancialExtension
    {
        public static FinanciallaunchDto MapToDto(this Financiallaunch entity)
        {
            return new FinanciallaunchDto
            {
                Id = entity.Id,
                BankAccount = entity.BankAccount,
                CoinType = entity.CoinType,
                CostCenter = entity.CostCenter,
                CreateDate = entity.CreateDate,
                Description = entity.Description,
                IdempotencyKey = entity.IdempotencyKey.ToString(),
                LaunchType = entity.LaunchType,
                NameCustomerSupplier = entity.NameCustomerSupplier,
                PaymentMethod = entity.PaymentMethod,
                Status = entity.Status,
                Value = entity.Value
            };
        }
    }
}
