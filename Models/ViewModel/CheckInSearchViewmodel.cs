using System.ComponentModel.DataAnnotations;

namespace VisiTrack.Models.ViewModel
{
    public class CheckInSearchViewmodel
    {
        

        public string? Name { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
            public IEnumerable<ReportRowViewModel>? Visits { get; set; }
    }

    }

