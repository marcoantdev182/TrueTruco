using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class NetworkManagerController : MonoBehaviour
    {
        public bool IsOnlineSessionActive { get; private set; }

        public void StartHost()
        {
            Debug.Log("[Network] StartHost requested. Install and wire Unity Netcode for GameObjects in this adapter.");
            IsOnlineSessionActive = true;
        }

        public void StartClient(string joinCode)
        {
            Debug.Log("[Network] StartClient requested with code '" + joinCode + "'. Relay transport wiring comes next.");
            IsOnlineSessionActive = true;
        }

        public void Shutdown()
        {
            Debug.Log("[Network] Shutdown requested.");
            IsOnlineSessionActive = false;
        }

        public void SyncPlayCard(SeatId seat, Card card)
        {
            Debug.Log("[Network] SyncPlayCard placeholder: " + SeatUtility.GetDisplayName(seat) + " -> " + card.DisplayName);
        }

        public void SyncSignal(SeatId seat, SignalType signal)
        {
            Debug.Log("[Network] SyncSignal placeholder: " + SeatUtility.GetDisplayName(seat) + " -> " + SignalDefinition.GetLabel(signal));
        }

        public void SyncTrucoAction(SeatId seat, string actionName)
        {
            Debug.Log("[Network] SyncTrucoAction placeholder: " + SeatUtility.GetDisplayName(seat) + " -> " + actionName);
        }
    }
}

