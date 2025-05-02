using Financial.Domain.Events;

namespace Financial.Common
{
    public class FinanciallaunchEvent : EventBase<Financiallaunch>
    {
        public FinanciallaunchEvent(Financiallaunch entity)
        {
            this.Entity = entity;
            this.Idempotency = entity.IdempotencyKey;
            this.Date = DateTime.UtcNow;
        }
    }
}
