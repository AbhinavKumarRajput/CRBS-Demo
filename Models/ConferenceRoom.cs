using System.ComponentModel.DataAnnotations;

namespace CRBS.Models
{
    public class ConferenceRoom
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
