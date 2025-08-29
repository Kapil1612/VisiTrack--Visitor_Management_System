using System.ComponentModel.DataAnnotations;

namespace VisiTrack.Models.ViewModel
{
    public class ReportViewModel
    {

            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
            public DateTime? StartDate { get; set; }
           
           
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
            public DateTime? EndDate { get; set; }

            public string? VisitorSearch { get; set; }
            public string? HostSearch { get; set; }
            public string? CompanySearch { get; set; }
            public string? PurposeSearch { get; set; }
            public string? StatusSearch{ get; set; }
           
        public int DailyCount { get; set; }
            public int CustomCount { get; set; }

        public List<ReportRowViewModel>? DailyReport { get; set; } 


            public List<ReportRowViewModel>? CustomReport { get; set; } 
        
    }
}
