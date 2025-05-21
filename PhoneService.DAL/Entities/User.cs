using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneService.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        [NotMapped]
        public string NewPassword { get; set; } = "";
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; } = "Client";
        public List<Request> Requests { get; set; } = [];
        public List<Request> Responses { get; set; } = [];
    }
}
