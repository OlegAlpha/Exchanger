using System.Collections.ObjectModel;
using ExchangerService.DataAccessLayer.CRUD;
using ExchangerService.DataAccessLayer.Entities;
using ExchangeService.BusinessLogic.Models.LocalAlternatives;

namespace ExchangeService.BusinessLogic.Models.Story;
public class UserStory
{
    public int Id { get; }
    private readonly BasicOperation _operation;
    public ObservableCollection<LocalExchangeStory> ExchangeStories { get; set; }

    public UserStory(int Id, BasicOperation operation)
    {
        this.Id = Id;
        ExchangeStories = new ObservableCollection<LocalExchangeStory>();
        _operation = operation;

        ExchangeStories.CollectionChanged += ExchangeStories_CollectionChanged;
    }

    private void ExchangeStories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                {

                    foreach (LocalExchangeStory item in e.NewItems)
                    {
                        ExchangeStory exchangeStory = new ExchangeStory()
                        {
                            UserId = Id,
                            Amount = item.Amount,
                            Rate = item.Rate,
                            ExchangeRateId = item.Rate.Id,
                            Created = DateTime.UtcNow,
                        };

                        _operation.AddAsync(exchangeStory);
                    }
                }
                break;
        }

    }
}
