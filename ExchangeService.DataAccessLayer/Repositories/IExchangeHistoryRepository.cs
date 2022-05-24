using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.DataAccessLayer.Repositories
{
    public interface IExchangeHistoryRepository
    {
        void Add(ExchangeHistory entity);
        IEnumerable<ExchangeHistory>? FindByUserIdOrDefault(int userId);
    }
}
