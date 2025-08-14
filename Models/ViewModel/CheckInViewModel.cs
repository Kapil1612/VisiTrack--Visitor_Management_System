using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace VisiTrack.Models.ViewModel
{
    public class CheckInViewModel
    {
        public Visitor Visitor { get; set; } = new Visitor();

        [Required]
        [Display(Name = "Host")]
        public int HostID { get; set; }

        public List<SelectListItem>? Hosts { get; set; }

        [Required]
        public string? Purpose { get; set; }


    }
}
