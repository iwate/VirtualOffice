using System.Linq;

namespace VirtualOffice.Models
{
    public class EmptyDeskDispatcher : IDeskDispatcher
    {
        public VirtualOfficeDesk Dispatch(VirtualOfficeStore store, string connectionId, string name)
        {
            return store.Desks.Where(d => !store.Users.Any(u => u.DeskId == d.Id)).OrderBy(desk => desk.R).FirstOrDefault();
        }
    }
}
