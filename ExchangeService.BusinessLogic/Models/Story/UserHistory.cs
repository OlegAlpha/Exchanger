using System.Collections.ObjectModel;
using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.DataAccessLayer.Entities;
using ExchangeService.DataAccessLayer.Repositories;

namespace ExchangeService.BusinessLogic.Models.Story;
public class UserHistory
{
    public int Id { get; }
    private readonly IExchangeHistoryRepository _repository;
    public ObservableCollection<LocalExchangeStory> ExchangeStories { get; set; }

    public UserHistory(int id, IExchangeHistoryRepository repository)
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

                    foreach (LocalExchangeHitory item in e.NewItems)
                    {
                        ExchangeHistory exchangeHistory = new ExchangeHistory()
                        {
                            UserId = Id,
                            Amount = (double)item.Amount,
                            Rate = item.Rate,
                            Created = DateTime.UtcNow,
                        };

                       _repository.Add(exchangeHistory);
                    }
                }
                break;
        }

    }
}
