using PhoneService.DAL.Models;

namespace PhoneService.DAL.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public StockType Type { get; set; }
        public int Discount { get; set; }
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }

    }
}
