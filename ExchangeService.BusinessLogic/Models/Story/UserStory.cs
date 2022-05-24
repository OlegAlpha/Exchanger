using System.Collections.ObjectModel;
using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;

namespace ExchangeService.BusinessLogic.Models.Story;
public class UserStory
{
    public int Id { get; }
    private readonly IExchangeHistoryRepository _repository;
    public ObservableCollection<LocalExchangeStory> ExchangeStories { get; set; }

    public UserStory(int id, IExchangeHistoryRepository repository)
    {
        this.Id = id;
        ExchangeStories = new ObservableCollection<LocalExchangeStory>();
        _repository = repository;

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
                            Created = DateTime.UtcNow,
                        };

                       _repository.Add(exchangeStory);
                    }
                }
                break;
        }

    }
}
