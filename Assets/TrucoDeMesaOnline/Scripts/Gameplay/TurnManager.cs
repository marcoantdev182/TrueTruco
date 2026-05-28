using System;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class TurnManager : MonoBehaviour
    {
        private readonly SeatId[] turnOrder =
        {
            SeatId.LocalPlayer,
            SeatId.RightRival,
            SeatId.Partner,
            SeatId.LeftRival
        };

        public event Action<SeatId> TurnChanged;

        public SeatId CurrentSeat { get; private set; }

        public bool IsLocalTurn
        {
            get { return CurrentSeat == SeatId.LocalPlayer; }
        }

        public void SetCurrentSeat(SeatId seat)
        {
            CurrentSeat = seat;
            TurnChanged?.Invoke(CurrentSeat);
        }

        public void Advance()
        {
            int currentIndex = Array.IndexOf(turnOrder, CurrentSeat);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int nextIndex = (currentIndex + 1) % turnOrder.Length;
            SetCurrentSeat(turnOrder[nextIndex]);
        }
    }
}

