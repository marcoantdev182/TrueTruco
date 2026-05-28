namespace TrucoDeMesaOnline
{
    public struct PlayedCard
    {
        public SeatId Seat;
        public Card Card;
        public int PlayOrder;

        public PlayedCard(SeatId seat, Card card, int playOrder)
        {
            Seat = seat;
            Card = card;
            PlayOrder = playOrder;
        }
    }
}

