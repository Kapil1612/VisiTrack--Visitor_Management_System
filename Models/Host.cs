using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VisiTrack.Models;


public class Host
{

    [Key]
    public int HostID { get; set; }

    [Required]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [StringLength(100)]
    public string? Department { get; set; }

    [Phone]
    [StringLength(20)]
    public string? ContactNumber { get; set; }


    public ICollection<Visit>? Visits { get; set; }
}

