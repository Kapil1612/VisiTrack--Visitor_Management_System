namespace VisiTrack.Models.ViewModel
{
    public class HostIndexViewModel
    {
        public Host NewHost { get; set; } = new Host();
        public List<HostViewModel>? HostList { get; set; }
        public List<Host>? HostLists { get; set; }
    }
}
