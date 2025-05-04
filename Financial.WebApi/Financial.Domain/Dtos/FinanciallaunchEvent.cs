using System.Diagnostics.Tracing;

namespace Financial.Domain.Dtos
{
    public class FinanciallaunchEvent : EventBase<Financiallaunch>
    {
        public FinanciallaunchEvent(Financiallaunch entity)
        {
            this.Entity = entity;
            this.Idempotency = entity.IdempotencyKey.ToString();
            this.Date = DateTime.UtcNow;
        }
    }
   
}
