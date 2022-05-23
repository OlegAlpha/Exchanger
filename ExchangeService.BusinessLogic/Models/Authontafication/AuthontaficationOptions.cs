using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Models;
internal class AuthontaficationOptions
{
    static readonly int lifeTime = 5;
    public string Source { get; set; }
    public int UserId { get; set; }
}
