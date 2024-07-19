using Microsoft.AspNetCore.SignalR;

namespace DealControllerService.HubConfig
{
    public class DealStatusHub :Hub
    {
        public async Task SendStatusUpdate(string dealId, string status)
        {
            await Clients.All.SendAsync("ReceiveStatusUpdate", dealId, status);
        }
    }
}
