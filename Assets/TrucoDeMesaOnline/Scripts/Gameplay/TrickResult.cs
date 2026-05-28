namespace TrucoDeMesaOnline
{
    public struct TrickResult
    {
        public bool IsValid;
        public SeatId WinningSeat;
        public TeamId WinningTeam;
        public int LocalTeamTricks;
        public int RivalTeamTricks;

        public TrickResult(SeatId winningSeat, TeamId winningTeam, int localTeamTricks, int rivalTeamTricks)
        {
            IsValid = true;
            WinningSeat = winningSeat;
            WinningTeam = winningTeam;
            LocalTeamTricks = localTeamTricks;
            RivalTeamTricks = rivalTeamTricks;
        }
    }
}

