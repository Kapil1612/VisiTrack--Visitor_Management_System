namespace VisiTrack.Models.ViewModel
{
    public class ReportRowViewModel
    {
        public int VisitID { get; set; }
        public string? VisitorName { get; set; }
        public string? Company { get; set; }
        public string? HostName { get; set; }
        public string? Purpose { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Status { get; set; }
        public string? Duration
        {
            get
            {
                if (CheckInTime.HasValue && CheckOutTime.HasValue)
                {
                    var duration = CheckOutTime.Value - CheckInTime.Value;
                    return $"{(int)duration.TotalHours}h {duration.Minutes}m";
                }
                return null;
            }
        }
        public List<Visit>? Visits { get; set; }

    }
}
