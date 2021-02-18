using pjfm.Models;

namespace pjfm.Services
{
    public interface IDjHubMessageService
    {
        void SendMessageToClient(HubServerMessage hubServerMessage);
    }
}