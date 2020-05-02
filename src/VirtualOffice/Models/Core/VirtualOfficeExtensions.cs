using System.Collections.Generic;
using System.Linq;

namespace VirtualOffice.Models
{
    public static class VirtualOfficeExtensions
    {
        public static VirtualOfficeDesk Find(this IEnumerable<VirtualOfficeDesk> desks, string id)
            => desks.Where(o => o.Id == id).FirstOrDefault();

        public static VirtualOfficeUser Find(this IEnumerable<VirtualOfficeUser> users, string id)
            => users.Where(o => o.ConnectionId == id).FirstOrDefault();
    }
}
