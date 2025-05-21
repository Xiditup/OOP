using PhoneService.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneService.DAL.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public ServiceCategory Category { get; set; }
        public List<Request> Requests { get; set; } = null!;
        public List<Stock> Stocks { get; set; } = null!;

        public int PriceWithDiscount {
            get
            {
                var discount = Stocks.FirstOrDefault(s => s.Type == StockType.Discount);
                if (discount?.Discount != null)
                {
                    return Price - (discount.Discount * Price / 100);
                }
                return Price;
            }
        }

        public int Discount
        {
            get
            {
                var discount = Stocks.FirstOrDefault(s => s.Type == StockType.Discount);
                if (discount?.Discount != null)
                {
                    return discount.Discount;
                }
                return 0;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
