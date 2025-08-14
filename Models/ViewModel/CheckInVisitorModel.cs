namespace VisiTrack.Models.ViewModel
{
    public class CheckInVisitorModel
    {
    
            public int VisitID { get; set; }
            public string? VisitorName { get; set; }
            public string? Company { get; set; }
            public string? HostName { get; set; }
            public DateTime CheckInTime { get; set; }

            public List<Visit>? Visits { get; set; }

    }
}
