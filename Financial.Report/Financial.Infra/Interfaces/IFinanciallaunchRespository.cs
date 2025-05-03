namespace Financial.Infra.Interfaces
{
    public interface IFinanciallaunchRespository
    {
        Task<decimal> GetAsync();
        Task SaveAsync(decimal value);
    }
}
