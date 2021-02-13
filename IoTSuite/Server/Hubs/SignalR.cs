using IoTSuite.Server.Services;
using IoTSuite.Server.Tasks;
using IoTSuite.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IoTSuite.Server.Hubs
{
    [Authorize]
    public class SignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }

        public async Task SendMessageToUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }

        public async Task SendMessageToThing(Guid ThingId, ThingActions thingAction)
        {
            await MQTTTask.PublishCommand(ThingId, thingAction);
        }

        public async Task GraphAPIGetUser()
        {
            var userId = Context
                            .User
                            .Claims
                            .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                            .First()
                            .Value
                            .ToLower();

            var user = await GraphApiService.GetUser(userId);

            await Clients.User(userId).SendAsync("GraphAPIGetUser", user);
        }
    }
}
