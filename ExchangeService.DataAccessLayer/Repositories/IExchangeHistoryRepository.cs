using ExchangeService.DataAccessLayer.Entities;

namespace ExchangeService.DataAccessLayer.Repositories
{
    public interface IExchangeHistoryRepository
    {
        void Add(ExchangeHistory entity);
        Task<IEnumerable<ExchangeHistory>> FindByUserIdOrDefault(int userId);
    }
}
