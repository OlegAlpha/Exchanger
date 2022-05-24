using System.Collections.ObjectModel;
using ExchangeService.BusinessLogic.Models.LocalAlternatives;
using ExchangeService.DataAccessLayer.CRUD;
using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.BusinessLogic.Models.Story;
public class UserHistory
{
    public int Id { get; }
    private readonly BasicOperation _operation;
    public ObservableCollection<LocalExchangeHitory> ExchangeStories { get; set; }

    public UserHistory(int Id, BasicOperation operation)
    {
        this.Id = Id;
        ExchangeStories = new ObservableCollection<LocalExchangeHitory>();
        _operation = operation;

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

                       _operation.Add(exchangeHistory);
                    }
                }
                break;
        }

    }
}
