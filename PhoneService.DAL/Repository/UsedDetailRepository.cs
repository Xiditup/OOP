using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Repository
{
    public class UsedDetailRepository : Repository<UsedDetail>
    {
        public UsedDetailRepository(ApplicationContext context) : base(context) { }

        public async Task<UsedDetail?> GetByNameForRequestAsync(string name, int requestId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.Name == name && d.RequestId == requestId);
        }
    }
}
