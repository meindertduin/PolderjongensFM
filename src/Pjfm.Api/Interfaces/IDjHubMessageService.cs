using Pjfm.Api.Models;

namespace Pjfm.Api.Interfaces
{
    public interface IDjHubMessageService
    {
        void SendMessageToClient(HubServerMessage hubServerMessage);
    }
}