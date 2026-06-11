using Microsoft.AspNetCore.SignalR;

namespace SalesCRM.WebAPI.Hubs;

public class PaymentHub : Hub
{
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }
}
