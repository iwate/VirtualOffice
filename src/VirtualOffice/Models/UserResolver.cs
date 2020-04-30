using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtualOffice.Models
{
    public class UserResolver : IUserResolver
    {
        private readonly ICollection<IUserResolver> _resolvers = new List<IUserResolver>();
        public async Task<UserInfo> ResolveAsync()
        {
            foreach (var resolver in _resolvers)
            {
                var info = await resolver.ResolveAsync();
                if (info != null)
                    return info;
            }

            return null;
        }
        public UserResolver Add(IUserResolver resolver)
        {
            _resolvers.Add(resolver);
            return this;
        }
    }
}
