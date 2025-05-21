using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;

namespace PhoneService.DAL.Repository
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(ApplicationContext context) : base(context) { }

        new public IList<Service> GetAll()
        {
            return _dbSet.Include(s => s.Stocks).ToList();
        }
        public IEnumerable<Service> GetServicesByNameAndCategory(string name, ServiceCategory serviceCategory)
        {
            return _dbSet.Where(d => d.Name.ToLower().Contains(name) && (d.Category == serviceCategory || serviceCategory == ServiceCategory.All)).ToList();
        }
    }
}
