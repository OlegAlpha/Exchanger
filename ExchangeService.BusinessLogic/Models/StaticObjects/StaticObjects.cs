using ExchangeService.BusinessLogic.Models.Story;

namespace ExchangeService.BusinessLogic.Models.StaticObjects;
public static class StaticObjects
{
    public static Dictionary<int, UserStory> Stories { get; } = new Dictionary<int, UserStory>();
}
