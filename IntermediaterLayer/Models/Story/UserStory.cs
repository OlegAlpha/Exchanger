using IntermediateLayer.Models.LocalivesAlternatives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Models;
internal class UserStory
{
    public int Id { get; set; }
    public List<LocalExchangeStory> ExchangeStories { get; set; }
}
