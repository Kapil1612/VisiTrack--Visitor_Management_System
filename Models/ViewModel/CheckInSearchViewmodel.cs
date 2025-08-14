namespace VisiTrack.Models.ViewModel
{
    public class CheckInSearchViewmodel
    {
        

        public string? Name { get; set; }
            public DateTime? Date { get; set; }
            public IEnumerable<ReportRowViewModel>? Visits { get; set; }
    }

    }

