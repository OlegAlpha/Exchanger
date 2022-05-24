using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangerService.DataAccessLayer.Entities;
public class ExchangeStory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    [ForeignKey(nameof(ExchangeRate))]
    public int ExchangeRateId { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
