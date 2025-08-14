namespace VisiTrack.Models.ViewModel
{
    public class ReportViewModel
    {
       
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }


            public List<ReportRowViewModel>? DailyReport { get; set; } 


            public List<ReportRowViewModel>? CustomReport { get; set; } 
        
    }
}
