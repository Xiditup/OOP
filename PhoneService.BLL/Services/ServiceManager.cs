using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class ServiceManager
    {
        private readonly ServiceRepository _serviceRepository;
        public ServiceManager(ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public async Task<Service> CreateNewService(string name, int price, ServiceCategory category)
        {
            var service = new Service
            {
                Name = name,
                Price = price,
                Category = category
            };

            ModelValidator.ValidateService(service);

            await _serviceRepository.AddAsync(service);
            return service;
        }

        public async Task DeleteServiceAsync(Service service)
        {
            await _serviceRepository.RemoveAsync(service);
        }

        public IList<Service> GetAllServices()
        {
            return _serviceRepository.GetAll();
        }

        public IEnumerable<Service> GetServicesWithNameAndCategory(string name, ServiceCategory serviceCategory)
        {
            return _serviceRepository.GetServicesByNameAndCategory(name, serviceCategory);
        }

        public async Task SaveChangesAsync(IEnumerable<Service> services)
        {
            await _serviceRepository.UpdateRangeAsync(services);
        }
    }
}
