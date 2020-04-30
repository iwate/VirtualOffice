using System.Threading.Tasks;

namespace VirtualOffice.Models
{
    public interface IUserResolver
    {
        Task<UserInfo> ResolveAsync();
    }
}
