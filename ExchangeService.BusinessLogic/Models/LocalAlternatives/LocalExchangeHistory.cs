using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.Models.LocalAlternatives;
public class LocalExchangeHistory
{
    public decimal Amount { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
