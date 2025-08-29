namespace VisiTrack.Models.ViewModel
{
    public class VisitsViewModel
    {
            public int VisitID { get; set; }
            public string? VisitorName { get; set; }
            public string? VistorContactNumber { get; set; }
            public string? Company { get; set; }
            public string? HostName { get; set; }
            public string? Purpose { get; set; }
            public DateTime? CheckInTime { get; set; }
            public DateTime? CheckOutTime { get; set; }
            public string? Status { get; set; }

    }
}
