using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class PlayerController : MonoBehaviour
    {
        public Seat LocalSeat { get; private set; }
        public Camera PlayerCamera { get; private set; }
        public CameraLookController LookController { get; private set; }

        public void InitializeLocalPlayer(Seat localSeat, Camera playerCamera)
        {
            LocalSeat = localSeat;
            PlayerCamera = playerCamera;

            LookController = PlayerCamera.GetComponent<CameraLookController>();
            if (LookController == null)
            {
                LookController = PlayerCamera.gameObject.AddComponent<CameraLookController>();
            }

            LookController.AttachToSeat(localSeat);
        }
    }
}

