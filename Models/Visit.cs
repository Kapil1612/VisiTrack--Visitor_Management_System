using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VisiTrack.Models;


public class Visit
{

    [Key]
    public int VisitID { get; set; }

    [Required]
    public int VisitorID { get; set; }
    public Visitor? Visitor { get; set; }

    [Required]
    public int HostID { get; set; }
    public Host? Host { get; set; }

    [Required]
    [StringLength(500)]
    public string? Purpose { get; set; }


    public DateTime? CheckInTime { get; set; } = DateTime.Now;

    public DateTime? CheckOutTime { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Checked In";

}

