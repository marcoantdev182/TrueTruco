namespace TrucoDeMesaOnline
{
    public static class SeatUtility
    {
        public static TeamId GetTeam(SeatId seat)
        {
            return seat == SeatId.LocalPlayer || seat == SeatId.Partner
                ? TeamId.LocalTeam
                : TeamId.RivalTeam;
        }

        public static string GetDisplayName(SeatId seat)
        {
            switch (seat)
            {
                case SeatId.LocalPlayer:
                    return "Voce";
                case SeatId.RightRival:
                    return "Bot Bia (rival direita)";
                case SeatId.Partner:
                    return "Bot Ana (parceiro)";
                case SeatId.LeftRival:
                    return "Bot Caio (rival esquerda)";
                default:
                    return seat.ToString();
            }
        }

        public static string GetShortName(SeatId seat)
        {
            switch (seat)
            {
                case SeatId.LocalPlayer:
                    return "VOCE";
                case SeatId.RightRival:
                    return "BIA\nR DIR";
                case SeatId.Partner:
                    return "ANA\nPARC";
                case SeatId.LeftRival:
                    return "CAIO\nR ESQ";
                default:
                    return seat.ToString().ToUpperInvariant();
            }
        }

        public static bool IsLocal(SeatId seat)
        {
            return seat == SeatId.LocalPlayer;
        }
    }
}
