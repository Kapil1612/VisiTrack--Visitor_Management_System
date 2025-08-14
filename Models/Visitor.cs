using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VisiTrack.Models;


public class Visitor
{

    [Key]
    public int VisitorID { get; set; }

    [Required]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [StringLength(255)]
    public string? Company { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }
    
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Visit>? Visits { get; set; }
}

