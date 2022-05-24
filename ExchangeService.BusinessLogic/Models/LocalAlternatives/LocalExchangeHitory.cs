using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.Models.LocalAlternatives;
public class LocalExchangeHitory
{
    public decimal Amount { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
