using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Data;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;

namespace PhoneService.DAL.Repository
{
    public class RequestRepository : Repository<Request>
    {
        public RequestRepository(ApplicationContext context) : base(context) { }

        public IEnumerable<Request> GetClientRequests(int clientId)
        {
            return _dbSet.Where(r => r.ClientId == clientId).ToList();
        }

        public IEnumerable<Request> GetClientClosedRequestsWithoutReview(int clientId)
        {
            return _dbSet.Where(r => r.ClientId == clientId && r.Status == RequestStatus.Closed && r.Review == null).ToList();
        }

        public async Task<IEnumerable<Request>> GetAllFiltered(
            User user,
            string requestName,
            string deviceName,
            Service? selectedService,
            RequestStatus selectedStatus,
            bool onlyActive,
            bool onlyWithoutMaster,
            bool onlyCancelRequired
        )
        {
            var query = _dbSet.AsQueryable();

            query = query.Where(r => r.Name.Contains(requestName));
            query = query.Where(r => r.Device.Contains(deviceName));
            if (selectedService != null)
            {
                query = query.Where(r => r.ServiceId == selectedService.Id);
            }
            if (selectedStatus != RequestStatus.All)
            {
                query = query.Where(r => r.Status == selectedStatus);
            }
            if (onlyActive)
            {
                query = query.Where(r => r.Status != RequestStatus.Canceled && r.Status != RequestStatus.Closed);
            }
            if (onlyWithoutMaster)
            {
                query = query.Where(r => r.MasterId == null);
            }
            if (onlyCancelRequired)
            {
                query = query.Where(r => r.CancelRequired == true);
            }
            if (user.Role == "Client")
            {
                query = query.Where(r => r.ClientId == user.Id);
            }

            return await query.ToListAsync();
        }

        public Request GetFullRequestById(int id)
        {
            return _dbSet.Include(r => r.Client).Include(r => r.Master).Include(r => r.Service).Include(r => r.UsedDetails).First(r => r.Id == id);
        }
    }
}
