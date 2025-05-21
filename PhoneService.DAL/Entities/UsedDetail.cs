namespace PhoneService.DAL.Entities
{
    public class UsedDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }
    }
}
