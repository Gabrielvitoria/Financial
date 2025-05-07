using System.ComponentModel;

namespace Financial.Domain
{
    public enum launchTypeEnum
    {
        Revenue = 1, // Receita (entrada de dinheiro)
        Expense = 2 // Despesa (saída de dinheiro). 
    }

    public enum launchPaymentMethodEnum
    {
        Cash = 1, // Dinheiro
        Card = 2, // Cartão
        Ticket = 3 // Boleto
    }

    public enum launchStatusEnum
    {
        Open = 1, // Aberto
        PaidOff = 2,  // Quitado
        Canceled = 3 // Cancelado
    }
}
