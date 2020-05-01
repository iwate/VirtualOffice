using System.Collections.Generic;

namespace VirtualOffice.Models
{
    public class Config
    {
        public string HostName { get; set; }
        public string SkyWayKey { get; set; }
        public string FloorImage { get; set; }
        public IEnumerable<VirtualOfficeDesk> DeskMap { get; set; }
        public IEnumerable<string> SlackWebHooks { get; set; }
        public string SlackWebHookTemplate { get; set; }
    }
}
