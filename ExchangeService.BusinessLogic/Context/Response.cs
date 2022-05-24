using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeService.BusinessLogic.Context;
internal class Response
{
    public string @base { get; set; }
    string date { get; set; }
    string start_date { get; set; }
    string end_date { get; set; }
    object rates { get; set; }
    bool success { get; set; } = true;
    bool fluctuation { get; set; }
    bool historical { get; set; }
    object info { get; set; }
    object query { get; set; }

    string timestamp { get; set; }
    string result { get; set; }
}
