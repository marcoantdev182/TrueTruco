using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class SeatManager : MonoBehaviour
    {
        private readonly Dictionary<SeatId, Seat> seatsById = new Dictionary<SeatId, Seat>();
        private readonly List<Seat> seats = new List<Seat>();

        private Transform seatsRoot;

        public IReadOnlyList<Seat> Seats
        {
            get { return seats; }
        }

        public bool HasSeats
        {
            get { return seats.Count == GameConstants.PlayerCount; }
        }

        public void BuildSeats(Transform parent)
        {
            ClearSeats();

            GameObject root = new GameObject("Seats");
            root.transform.SetParent(parent, false);
            seatsRoot = root.transform;

            CreateSeat(SeatId.LocalPlayer, new Vector3(0f, 0f, -GameConstants.SeatRadius));
            CreateSeat(SeatId.RightRival, new Vector3(GameConstants.SeatRadius, 0f, 0f));
            CreateSeat(SeatId.Partner, new Vector3(0f, 0f, GameConstants.SeatRadius));
            CreateSeat(SeatId.LeftRival, new Vector3(-GameConstants.SeatRadius, 0f, 0f));
        }

        public Seat GetSeat(SeatId seatId)
        {
            seatsById.TryGetValue(seatId, out Seat seat);
            return seat;
        }

        private void CreateSeat(SeatId seatId, Vector3 position)
        {
            GameObject seatObject = new GameObject(SeatUtility.GetDisplayName(seatId) + " Seat");
            seatObject.transform.SetParent(seatsRoot, false);
            seatObject.transform.position = position;

            Vector3 lookDirection = Vector3.zero - position;
            lookDirection.y = 0f;
            seatObject.transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);

            Seat seat = seatObject.AddComponent<Seat>();
            seat.Initialize(seatId);

            seatsById.Add(seatId, seat);
            seats.Add(seat);
        }

        private void ClearSeats()
        {
            seatsById.Clear();
            seats.Clear();

            if (seatsRoot != null)
            {
                DestroyObject(seatsRoot.gameObject);
                seatsRoot = null;
            }
        }

        private static void DestroyObject(GameObject target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
    }
}

