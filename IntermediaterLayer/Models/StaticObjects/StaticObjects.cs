using DataBaseLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Models.StaticObjects;
public static class StaticObjects
{
    public static Dictionary<int, UserStory> Stories { get; } = new Dictionary<int, UserStory>();
}
