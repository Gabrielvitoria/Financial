namespace Financial.Domain.Events
{
    public class Financiallaunch
    {
        public Guid Id { get; set; }
        public string IdempotencyKey { get; set; }
        public int LaunchType { get; set; }
        public int PaymentMethod { get; set; }
        public int Status { get; set; }
        public string CoinType { get; set; }
        public decimal Value { get; set; }
        public string BankAccount { get; set; }
        public string NameCustomerSupplier { get; set; }
        public string CostCenter { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
