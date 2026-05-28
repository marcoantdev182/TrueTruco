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
                    return "Rival direita";
                case SeatId.Partner:
                    return "Parceiro";
                case SeatId.LeftRival:
                    return "Rival esquerda";
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
                    return "R DIR";
                case SeatId.Partner:
                    return "PARC";
                case SeatId.LeftRival:
                    return "R ESQ";
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

