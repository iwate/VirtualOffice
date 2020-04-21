namespace VirtualOffice.Models
{
    public interface IDeskDispatcher
    {
        VirtualOfficeDesk Dispatch(VirtualOfficeStore store, string connectionId, string name);
    }
}
