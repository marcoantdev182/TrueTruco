using System;

namespace TrucoDeMesaOnline
{
    [Serializable]
    public struct Card : IEquatable<Card>
    {
        public Rank Rank;
        public Suit Suit;

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public string ShortName
        {
            get { return GetRankShortName(Rank) + GetSuitShortName(Suit); }
        }

        public string DisplayName
        {
            get { return GetRankName(Rank) + " de " + GetSuitName(Suit); }
        }

        public bool Equals(Card other)
        {
            return Rank == other.Rank && Suit == other.Suit;
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Rank * 397) ^ (int)Suit;
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static string GetRankShortName(Rank rank)
        {
            switch (rank)
            {
                case Rank.Four:
                    return "4";
                case Rank.Five:
                    return "5";
                case Rank.Six:
                    return "6";
                case Rank.Seven:
                    return "7";
                case Rank.Queen:
                    return "Q";
                case Rank.Jack:
                    return "J";
                case Rank.King:
                    return "K";
                case Rank.Ace:
                    return "A";
                case Rank.Two:
                    return "2";
                case Rank.Three:
                    return "3";
                default:
                    return "?";
            }
        }

        public static string GetRankName(Rank rank)
        {
            switch (rank)
            {
                case Rank.Four:
                    return "4";
                case Rank.Five:
                    return "5";
                case Rank.Six:
                    return "6";
                case Rank.Seven:
                    return "7";
                case Rank.Queen:
                    return "Dama";
                case Rank.Jack:
                    return "Valete";
                case Rank.King:
                    return "Rei";
                case Rank.Ace:
                    return "As";
                case Rank.Two:
                    return "2";
                case Rank.Three:
                    return "3";
                default:
                    return "Carta";
            }
        }

        public static string GetSuitShortName(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return "P";
                case Suit.Hearts:
                    return "C";
                case Suit.Spades:
                    return "E";
                case Suit.Diamonds:
                    return "O";
                default:
                    return "?";
            }
        }

        public static string GetSuitName(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return "Paus";
                case Suit.Hearts:
                    return "Copas";
                case Suit.Spades:
                    return "Espadas";
                case Suit.Diamonds:
                    return "Ouros";
                default:
                    return "Naipe";
            }
        }
    }
}

