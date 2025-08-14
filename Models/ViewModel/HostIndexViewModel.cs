namespace VisiTrack.Models.ViewModel
{
    public class HostIndexViewModel
    {
        public Host NewHost { get; set; } = new Host();
        public List<Host>? HostList { get; set; }
    }
}
