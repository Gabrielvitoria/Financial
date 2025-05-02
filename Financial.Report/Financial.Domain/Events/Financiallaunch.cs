namespace Financial.Domain.Events
{
    public class Financiallaunch
    {
        public string IdempotencyKey { get; private set; }
        public string LaunchType { get; private set; }
        public string PaymentMethod { get; private set; }
        public string Status { get; private set; }
        public string CoinType { get; private set; }
        public decimal Value { get; private set; }
        public string BankAccount { get; private set; }
        public string NameCustomerSupplier { get; private set; }
        public string CostCenter { get; private set; }
        public string Description { get; private set; }
    }
}
