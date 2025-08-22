using System.ComponentModel.DataAnnotations;

namespace VisiTrack.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(500)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
