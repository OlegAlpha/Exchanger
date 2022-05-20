using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseLayer.Entities;

namespace IntermediateLayer.Models.LocalivesAlternatives;
internal class LocalExchangeStory
{
    public decimal Amount { get; set; }
    public ExchangeRate Rate { get; set; }
    public DateTime Created { get; set; }
}
