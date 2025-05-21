using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneService.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Mark { get; set; }
        public string Description { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int RequestId { get; set; }
        public Request Request { get; set; } = null!;
        [NotMapped]
        public bool IsOwner { get; set; } = false;
        [NotMapped]
        public bool IsEditing { get; set; } = false;
        [NotMapped]
        public string EditText { get; set; } = "Изменить";
    }
}
