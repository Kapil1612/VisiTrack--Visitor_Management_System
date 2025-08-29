using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VisiTrack.Models;


public class Host
{

    [Key]
    public int HostID { get; set; }
 
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "Contact Number is required")]
    [Phone(ErrorMessage = "Enter a valid phone number")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Contact Number must be between 10 and 15 characters")]
    [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "Contact Number must contain only digits")]
    [Display(Name = "Contact Number (Mobile)")]
    public string? ContactNumber { get; set; }


    public ICollection<Visit>? Visits { get; set; }
}

