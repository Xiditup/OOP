using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Repository
{
    public class DetailRepository : Repository<Detail>
    {
        public DetailRepository(ApplicationContext context) : base(context) { }

        public IEnumerable<Detail> GetDetailsByName(string name)
        {
            return _dbSet.Where(d => d.Name.ToLower().Contains(name)).ToList();
        }
    }
}
