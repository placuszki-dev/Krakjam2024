using System.Collections.Generic;
using System.Threading.Tasks;

namespace Placuszki.Krakjam2024.Server
{
    public interface IGameHub
    {
        Task SendDataPacket(DataPacket dataPacket);
        Task SendUserInfo(UserInfo userInfo);
    }
}