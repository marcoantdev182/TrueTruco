namespace TrucoDeMesaOnline
{
    public static class CardValueResolver
    {
        public static Rank GetManilhaRank(Card vira)
        {
            int next = ((int)vira.Rank + 1) % 10;
            return (Rank)next;
        }

        public static bool IsManilha(Card card, Card vira)
        {
            return card.Rank == GetManilhaRank(vira);
        }

        public static int GetPower(Card card, Card vira)
        {
            if (IsManilha(card, vira))
            {
                return 100 + GetManilhaSuitPower(card.Suit);
            }

            return (int)card.Rank;
        }

        public static int Compare(Card left, Card right, Card vira)
        {
            int leftPower = GetPower(left, vira);
            int rightPower = GetPower(right, vira);
            return leftPower.CompareTo(rightPower);
        }

        private static int GetManilhaSuitPower(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return 4;
                case Suit.Hearts:
                    return 3;
                case Suit.Spades:
                    return 2;
                case Suit.Diamonds:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}

