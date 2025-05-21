using PhoneService.DAL.Models;

namespace PhoneService.DAL.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Device { get; set; }
        public List<string> ImagePathes { get; set; } = [];
        public bool CancelRequired { get; set; }
        public RequestStatus Status { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int Cost { get; set; }
        public List<UsedDetail> UsedDetails { get; set; } = [];
        public string Response { get; set; }
        public int? MasterId { get; set; }
        public User? Master { get; set; }
        public int ClientId { get; set; }
        public User Client { get; set; }
        public int? ReviewId { get; set; }
        public Review? Review { get; set; }
    }
}
