using Placuszki.Krakjam2024.Server;

namespace CargoAR.Shared.Network
{
    public interface IClientHub
    {
        void SendEndGameToServer(int winningCheeseType);
        void SendMainMenuOpenedToServer();
        void VibratePhone(string playerId, float force);
    }
}