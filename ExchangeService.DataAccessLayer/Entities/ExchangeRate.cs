using System.ComponentModel.DataAnnotations;

namespace ExchangeService.DataAccessLayer.Entities;
public class ExchangeRate
{
    [Key]
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public double Rate { get; set; }
    public DateTime? Date { get; set; }
}
