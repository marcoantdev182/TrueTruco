using System;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class ScoreManager : MonoBehaviour
    {
        private readonly int[] scores = new int[2];

        public event Action ScoresChanged;

        public int CurrentRoundValue { get; private set; } = 1;

        public int LocalTeamScore
        {
            get { return scores[(int)TeamId.LocalTeam]; }
        }

        public int RivalTeamScore
        {
            get { return scores[(int)TeamId.RivalTeam]; }
        }

        public void ResetMatch()
        {
            scores[(int)TeamId.LocalTeam] = 0;
            scores[(int)TeamId.RivalTeam] = 0;
            ResetRoundValue();
            ScoresChanged?.Invoke();
        }

        public void ResetRoundValue()
        {
            CurrentRoundValue = 1;
            ScoresChanged?.Invoke();
        }

        public void AwardRound(TeamId team)
        {
            scores[(int)team] = Mathf.Min(GameConstants.PointsToWin, scores[(int)team] + CurrentRoundValue);
            ScoresChanged?.Invoke();
        }

        public int RequestTruco(SeatId requester)
        {
            CurrentRoundValue = GetNextRoundValue(CurrentRoundValue);
            Debug.Log("[Truco] " + SeatUtility.GetDisplayName(requester) + " pediu Truco. Rodada agora vale " + CurrentRoundValue + ".");
            ScoresChanged?.Invoke();
            return CurrentRoundValue;
        }

        public TeamId Run(SeatId runner)
        {
            TeamId winner = SeatUtility.GetTeam(runner) == TeamId.LocalTeam ? TeamId.RivalTeam : TeamId.LocalTeam;
            AwardRound(winner);
            Debug.Log("[Truco] " + SeatUtility.GetDisplayName(runner) + " correu. Ponto para " + GetTeamLabel(winner) + ".");
            return winner;
        }

        public bool HasMatchWinner(out TeamId winner)
        {
            if (LocalTeamScore >= GameConstants.PointsToWin)
            {
                winner = TeamId.LocalTeam;
                return true;
            }

            if (RivalTeamScore >= GameConstants.PointsToWin)
            {
                winner = TeamId.RivalTeam;
                return true;
            }

            winner = TeamId.LocalTeam;
            return false;
        }

        public static string GetTeamLabel(TeamId team)
        {
            return team == TeamId.LocalTeam ? "sua dupla" : "dupla rival";
        }

        private static int GetNextRoundValue(int current)
        {
            if (current < 3)
            {
                return 3;
            }

            if (current < 6)
            {
                return 6;
            }

            if (current < 9)
            {
                return 9;
            }

            return 12;
        }
    }
}

