using System.ComponentModel.DataAnnotations;

namespace ExchangerService.DataAccessLayer.Entities;
public class ExchangeRate
{
    [Key]
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public decimal Rate { get; set; }
    public DateTime? Date { get; set; }
    public object CachedResponse { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To);
    }
}
