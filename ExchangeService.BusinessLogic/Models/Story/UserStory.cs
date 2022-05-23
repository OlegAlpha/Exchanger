using DataBaseLayer.CRUD;
using DataBaseLayer.Entities;
using IntermediateLayer.Models.LocalivesAlternatives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Models;
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
