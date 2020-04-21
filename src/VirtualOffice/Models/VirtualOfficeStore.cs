using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VirtualOffice.Models
{
    public class VirtualOfficeStore
    {
        private static readonly ConcurrentDictionary<string, VirtualOfficeUser> _users = new ConcurrentDictionary<string, VirtualOfficeUser>();
        public IEnumerable<VirtualOfficeUser> Users => _users.Values;
        public IEnumerable<VirtualOfficeDesk> Desks { get; }

        private readonly IDeskDispatcher _deskDispatcher;

        public VirtualOfficeStore(IOptions<Config> config, IDeskDispatcher deskDispatcher)
        {
            Desks = config.Value.DeskMap ?? Enumerable.Empty<VirtualOfficeDesk>();
            _deskDispatcher = deskDispatcher;
        }

        private static object @lock = new object();
        public VirtualOfficeUser AddUser(string connectionId, string name, string icon)
        {
            lock (@lock)
            {
                var desk = _deskDispatcher.Dispatch(this, connectionId, name);

                if (desk == null)
                    return null;

                var user = new VirtualOfficeUser
                {
                    ConnectionId = connectionId,
                    Name = name,
                    Icon = icon,
                    DeskId = desk.Id
                };
                return _users.TryAdd(connectionId, user) ? user : null;
            }
        }

        public void MoveUser(string connectionId, string deskId)
        {
            if (_users.TryGetValue(connectionId, out var current))
            {
                _users.TryUpdate(connectionId, new VirtualOfficeUser
                {
                    ConnectionId = current.ConnectionId,
                    Name = current.Name,
                    Icon = current.Icon,
                    DeskId = deskId
                }, current);
            }
        }

        public VirtualOfficeUser RemoveUser(string connectionId)
        {
            return _users.TryRemove(connectionId, out var user) ? user : null;
        }
    }
}


