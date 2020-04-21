using System.Collections.Generic;

namespace VirtualOffice.Models
{
    public class Config
    {
        public string SkyWayKey { get; set; }
        public string FloorImage { get; set; }
        public IEnumerable<VirtualOfficeDesk> DeskMap { get; set; }
    }
}
