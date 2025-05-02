namespace Financial.Domain.Dtos
{
    public class EventBase<T>
    {
        public T Entity { get; set; }
        public string Idempotency { get; set; }
        public DateTime Date { get; set; }
    }
}
