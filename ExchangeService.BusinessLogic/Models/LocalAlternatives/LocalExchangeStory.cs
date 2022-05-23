using ExchangerService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.Models.LocalAlternatives;
public class LocalExchangeStory
{
    public decimal Amount { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
