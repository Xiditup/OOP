using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Repository
{
    public class StockRepository : Repository<Stock>
    {
        public StockRepository(ApplicationContext context) : base(context) { }
        new public IList<Stock> GetAll()
        {
            return _dbSet.OrderByDescending(s => s.Id).ToList();
        }
    }
}
