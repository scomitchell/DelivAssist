using Microsoft.AspNetCore.SignalR;

namespace GigBoard.Hubs
{
    public class StatisticsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await base.OnConnectedAsync();
        }
    }
}