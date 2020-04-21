using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualOffice.Models
{
    public partial class VirtualOfficeHub : Hub
    {
        private readonly VirtualOfficeStore _store = null;
        public VirtualOfficeHub(VirtualOfficeStore store)
        {
            _store = store;
        }

        public void Reflesh()
        {
            Clients.All.SendAsync("Reflesh", _store.Users).ConfigureAwait(false);
        }

        public Result EnterUser(string name, string icon)
        {
            var user = _store.Users.Find(Context.ConnectionId);
            if (user != null)
                return Result.CreateFaild("AlreadyEntered");

            var added = _store.AddUser(Context.ConnectionId, name, icon ?? $"https://ui-avatars.com/api/?name={name}&size=48");
            if (added == null)
                return Result.CreateFaild("Full");

            Reflesh();

            return Result.CreateSucceeded();
        }

        public Result MoveUser(string deskId)
        {
            var user = _store.Users.Find(Context.ConnectionId);

            if (user == null)
                return Result.CreateFaild("NotFound");

            var desk = _store.Desks.Find(deskId);

            if (desk == null)
                return Result.CreateFaild("NotFound");

            _store.MoveUser(Context.ConnectionId, deskId);

            Reflesh();

            return Result.CreateSucceeded();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Desks", _store.Desks);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _store.RemoveUser(Context.ConnectionId);
            Reflesh();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
