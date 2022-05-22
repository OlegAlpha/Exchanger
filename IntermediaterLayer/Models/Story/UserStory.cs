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
    public ObservableCollection<LocalExchangeStory> ExchangeStories { get; set; }

    public UserStory(int Id)
    {
        this.Id = Id;
        ExchangeStories = new ObservableCollection<LocalExchangeStory>();

        ExchangeStories.CollectionChanged += ExchangeStories_CollectionChanged;
    }

    private void ExchangeStories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                {
                    BasicOperation operations = new BasicOperation();

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

                        operations.AddAsync(exchangeStory);
                    }
                }
                break;
        }

    }
}
