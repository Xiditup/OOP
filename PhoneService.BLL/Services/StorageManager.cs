using PhoneService.DAL.Entities;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class StorageManager
    {
        private readonly DetailRepository _detailRepository;
        public StorageManager(DetailRepository detailRepository)
        {
            _detailRepository = detailRepository;
        }
        public IEnumerable<Detail> GetAllDetails()
        {
            return _detailRepository.GetAll();
        }

        public IEnumerable<Detail> GetDetailsByName(string name)
        {
            return _detailRepository.GetDetailsByName(name);
        }

        public async Task<Detail> CreateDetail(string name, int quantity)
        {
            var detail = new Detail { Name = name, Quantity = quantity };
            ModelValidator.ValidateDetail(detail);

            await _detailRepository.AddAsync(detail);
            return detail;
        }

        public async Task SaveChangesAsync(IEnumerable<Detail> details)
        {
            foreach (var detail in details)
            {
                ModelValidator.ValidateDetail(detail);
                await _detailRepository.UpdateAsync(detail);
            }
        }

        public async Task DeleteDetailAsync(Detail detail)
        {
            await _detailRepository.RemoveAsync(detail);
        }
    }
}
