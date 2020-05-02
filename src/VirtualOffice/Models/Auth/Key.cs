using System;

namespace VirtualOffice.Models
{
    public class Key
    {
        public string Code { get; set; }
        public DateTimeOffset ExpiresUtc { get; set; }
    }
}
