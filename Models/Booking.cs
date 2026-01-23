using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRBS.Models
{
    public class Booking
    {
       [Key]
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }

        [Required]
        public TimeOnly FromTime { get; set; }

        [Required]
        public TimeOnly ToTime { get; set; }

        public string Status { get; set; } = "Pending";

        // 🔹 Foreign Keys
        [Required]
        public string UserId { get; set; }

        // 🔹 Navigation Properties
        [ForeignKey(nameof(RoomId))]
        [ValidateNever]
        public ConferenceRoom Room { get; set; }

        [ForeignKey(nameof(UserId))]
        [ValidateNever]
        public AppUser User { get; set; }
    }
}
