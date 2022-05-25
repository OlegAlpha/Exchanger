﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeService.DataAccessLayer.Entities;
public class ExchangeHistory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Amount { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
