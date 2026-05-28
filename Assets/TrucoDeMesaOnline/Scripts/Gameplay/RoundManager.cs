using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class RoundManager : MonoBehaviour
    {
        private readonly Dictionary<SeatId, List<Card>> handsBySeat = new Dictionary<SeatId, List<Card>>();
        private readonly List<PlayedCard> currentTrick = new List<PlayedCard>();
        private readonly int[] tricksWonByTeam = new int[2];

        public Card Vira { get; private set; }
        public bool IsRoundActive { get; private set; }
        public int TricksPlayed { get; private set; }

        public int CurrentTrickCount
        {
            get { return currentTrick.Count; }
        }

        public void StartRound(Dictionary<SeatId, List<Card>> dealtHands, Card vira)
        {
            handsBySeat.Clear();
            currentTrick.Clear();
            tricksWonByTeam[0] = 0;
            tricksWonByTeam[1] = 0;
            TricksPlayed = 0;
            Vira = vira;
            IsRoundActive = true;

            foreach (KeyValuePair<SeatId, List<Card>> entry in dealtHands)
            {
                handsBySeat[entry.Key] = new List<Card>(entry.Value);
            }
        }

        public IReadOnlyList<Card> GetHand(SeatId seat)
        {
            if (!handsBySeat.TryGetValue(seat, out List<Card> hand))
            {
                return new List<Card>();
            }

            return hand;
        }

        public bool TryPlayCard(SeatId seat, int handIndex, out PlayedCard playedCard)
        {
            playedCard = new PlayedCard();

            if (!IsRoundActive)
            {
                return false;
            }

            if (!handsBySeat.TryGetValue(seat, out List<Card> hand))
            {
                return false;
            }

            if (handIndex < 0 || handIndex >= hand.Count)
            {
                return false;
            }

            Card card = hand[handIndex];
            hand.RemoveAt(handIndex);

            playedCard = new PlayedCard(seat, card, currentTrick.Count);
            currentTrick.Add(playedCard);
            return true;
        }

        public TrickResult ResolveCurrentTrick()
        {
            if (currentTrick.Count == 0)
            {
                return new TrickResult();
            }

            PlayedCard best = currentTrick[0];
            for (int i = 1; i < currentTrick.Count; i++)
            {
                if (CardValueResolver.Compare(currentTrick[i].Card, best.Card, Vira) > 0)
                {
                    best = currentTrick[i];
                }
            }

            TeamId winningTeam = SeatUtility.GetTeam(best.Seat);
            tricksWonByTeam[(int)winningTeam]++;
            TricksPlayed++;
            currentTrick.Clear();

            return new TrickResult(
                best.Seat,
                winningTeam,
                tricksWonByTeam[(int)TeamId.LocalTeam],
                tricksWonByTeam[(int)TeamId.RivalTeam]);
        }

        public bool TryGetRoundWinner(out TeamId winningTeam)
        {
            if (tricksWonByTeam[(int)TeamId.LocalTeam] >= 2)
            {
                winningTeam = TeamId.LocalTeam;
                IsRoundActive = false;
                return true;
            }

            if (tricksWonByTeam[(int)TeamId.RivalTeam] >= 2)
            {
                winningTeam = TeamId.RivalTeam;
                IsRoundActive = false;
                return true;
            }

            if (TricksPlayed >= GameConstants.CardsPerPlayer)
            {
                winningTeam = tricksWonByTeam[(int)TeamId.LocalTeam] >= tricksWonByTeam[(int)TeamId.RivalTeam]
                    ? TeamId.LocalTeam
                    : TeamId.RivalTeam;
                IsRoundActive = false;
                return true;
            }

            winningTeam = TeamId.LocalTeam;
            return false;
        }

        public void EndRound()
        {
            currentTrick.Clear();
            IsRoundActive = false;
        }
    }
}
