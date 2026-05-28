using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class Seat : MonoBehaviour
    {
        public SeatId SeatId { get; private set; }
        public TeamId TeamId { get; private set; }
        public Transform CameraAnchor { get; private set; }
        public Transform AvatarAnchor { get; private set; }
        public Transform LabelAnchor { get; private set; }

        public void Initialize(SeatId seatId)
        {
            SeatId = seatId;
            TeamId = SeatUtility.GetTeam(seatId);

            CameraAnchor = CreateAnchor("Camera Anchor", new Vector3(0f, 1.45f, 0f));
            AvatarAnchor = CreateAnchor("Avatar Anchor", new Vector3(0f, 0f, 0f));
            LabelAnchor = CreateAnchor("Label Anchor", new Vector3(0f, 2.05f, 0f));
        }

        private Transform CreateAnchor(string anchorName, Vector3 localPosition)
        {
            GameObject anchor = new GameObject(anchorName);
            anchor.transform.SetParent(transform, false);
            anchor.transform.localPosition = localPosition;
            anchor.transform.localRotation = Quaternion.identity;
            anchor.transform.localScale = Vector3.one;
            return anchor.transform;
        }
    }
}

