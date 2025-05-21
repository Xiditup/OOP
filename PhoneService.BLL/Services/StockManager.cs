using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class StockManager
    {
        private readonly StockRepository _stockRepository;
        private readonly ServiceManager _serviceManager;
        private readonly ImageService _imageService;
        public StockManager(StockRepository stockRepository, ServiceManager serviceManager, ImageService imageService)
        {
            _stockRepository = stockRepository;
            _serviceManager = serviceManager;
            _imageService = imageService;
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockRepository.GetAll();
        }
        public IEnumerable<Service> GetAllServices()
        {
            return _serviceManager.GetAllServices();
        }

        public async Task<Stock> CreateStock(
            string name, 
            string description, 
            FileInfo image, 
            StockType type, 
            int percentage, 
            Service? service,
            bool withoutService
        )
        {
            if (!withoutService)
            {
                if (service == null)
                {
                    throw new ArgumentException("Необходимо выбрать услугу");
                }
                if (service.Stocks.Any(s => s.Type == StockType.Discount) && type == StockType.Discount)
                {
                    throw new ArgumentException("Услуга не может иметь более 1 скидки");
                }
            }
            if (image == null)
            {
                throw new ArgumentException("Выберите фото");
            }
            var stock = new Stock
            {
                Name = name,
                Description = description,
                ImagePath = _imageService.Upload(image),
                Discount = percentage,
                Type = type,
                ServiceId = withoutService ? null : service.Id,
                Service = withoutService ? null : service
            };


            ModelValidator.ValidateStock(stock);
            if (!withoutService)
            {
                service.Stocks.Add(stock);
            }
            await _stockRepository.AddAsync(stock);
            return stock;
        }

        public async Task SaveChangesAsync(IEnumerable<Stock> discounts)
        {
            foreach (var discount in discounts)
            {
                ModelValidator.ValidateStock(discount);
                await _stockRepository.UpdateAsync(discount);
            }
        }

        public async Task DeleteStockAsync(Stock discount)
        {
            await _stockRepository.RemoveAsync(discount);
        }
    }
}
