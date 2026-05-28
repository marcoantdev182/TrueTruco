namespace TrucoDeMesaOnline
{
    public struct SignalEvent
    {
        public SeatId SourceSeat;
        public SignalType Signal;
        public float ServerTime;

        public SignalEvent(SeatId sourceSeat, SignalType signal, float serverTime)
        {
            SourceSeat = sourceSeat;
            Signal = signal;
            ServerTime = serverTime;
        }
    }
}

