using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Repository
{
    public class ReviewRepository : Repository<Review>
    {
        public ReviewRepository(ApplicationContext context) : base(context) { }

        new public IEnumerable<Review> GetAll()
        {
            return _dbSet.Include(r => r.User).Include(r => r.Request).ToList();
        }
    }
}
