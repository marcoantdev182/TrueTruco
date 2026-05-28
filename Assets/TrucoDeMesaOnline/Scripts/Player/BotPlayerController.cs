using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class BotPlayerController : MonoBehaviour
    {
        [SerializeField] private SeatId seatId;
        [SerializeField] private BotPersonality personality;

        public SeatId SeatId
        {
            get { return seatId; }
        }

        public string DisplayName
        {
            get { return SeatUtility.GetDisplayName(seatId); }
        }

        public void Configure(SeatId seat, BotPersonality botPersonality)
        {
            seatId = seat;
            personality = botPersonality;
            gameObject.name = "Bot - " + SeatUtility.GetDisplayName(seat);
        }

        public int ChooseCardIndex(RoundManager roundManager)
        {
            IReadOnlyList<Card> hand = roundManager.GetHand(seatId);
            if (hand.Count == 0)
            {
                return -1;
            }

            if (hand.Count == 1 || roundManager.CurrentTrickCount == 0)
            {
                return ChooseOpeningCard(hand, roundManager);
            }

            if (!roundManager.TryGetCurrentTrickLeader(out PlayedCard leader))
            {
                return ChooseWeakest(hand, roundManager.Vira);
            }

            TeamId botTeam = SeatUtility.GetTeam(seatId);
            TeamId leaderTeam = SeatUtility.GetTeam(leader.Seat);
            if (leaderTeam == botTeam)
            {
                return ChooseWeakest(hand, roundManager.Vira);
            }

            int winningIndex = ChooseWeakestWinningCard(hand, leader.Card, roundManager.Vira);
            if (winningIndex >= 0)
            {
                return winningIndex;
            }

            return ChooseWeakest(hand, roundManager.Vira);
        }

        public bool ShouldSignal(RoundManager roundManager)
        {
            IReadOnlyList<Card> hand = roundManager.GetHand(seatId);
            if (hand.Count == 0)
            {
                return false;
            }

            float chance = personality == BotPersonality.Partner ? 0.78f : 0.38f;
            return Random.value <= chance;
        }

        public SignalType ChooseSignal(RoundManager roundManager)
        {
            IReadOnlyList<Card> hand = roundManager.GetHand(seatId);
            float truthChance = personality == BotPersonality.Partner ? 0.76f : 0.52f;

            if (Random.value <= truthChance && TryChooseTrueSignal(hand, roundManager.Vira, out SignalType trueSignal))
            {
                return trueSignal;
            }

            return GetRandomSignal();
        }

        public bool ShouldAskTruco(RoundManager roundManager, int currentRoundValue)
        {
            if (currentRoundValue >= 3)
            {
                return false;
            }

            IReadOnlyList<Card> hand = roundManager.GetHand(seatId);
            if (hand.Count == 0)
            {
                return false;
            }

            int strength = 0;
            for (int i = 0; i < hand.Count; i++)
            {
                int power = CardValueResolver.GetPower(hand[i], roundManager.Vira);
                if (power >= 100)
                {
                    strength += 5;
                }
                else if (hand[i].Rank == Rank.Three)
                {
                    strength += 3;
                }
                else if (hand[i].Rank == Rank.Two || hand[i].Rank == Rank.Ace)
                {
                    strength += 2;
                }
            }

            float chance = personality == BotPersonality.BoldRival ? 0.4f : 0.18f;
            return strength >= 6 && Random.value <= chance;
        }

        private int ChooseOpeningCard(IReadOnlyList<Card> hand, RoundManager roundManager)
        {
            int ownTricks = roundManager.GetTricksWon(SeatUtility.GetTeam(seatId));
            int rivalTricks = roundManager.GetTricksWon(GetOpposingTeam());

            if (rivalTricks > ownTricks)
            {
                return ChooseStrongest(hand, roundManager.Vira);
            }

            if (personality == BotPersonality.BoldRival && Random.value < 0.45f)
            {
                return ChooseStrongest(hand, roundManager.Vira);
            }

            return ChooseWeakest(hand, roundManager.Vira);
        }

        private TeamId GetOpposingTeam()
        {
            return SeatUtility.GetTeam(seatId) == TeamId.LocalTeam ? TeamId.RivalTeam : TeamId.LocalTeam;
        }

        private static int ChooseWeakest(IReadOnlyList<Card> hand, Card vira)
        {
            int bestIndex = 0;
            int bestPower = CardValueResolver.GetPower(hand[0], vira);

            for (int i = 1; i < hand.Count; i++)
            {
                int power = CardValueResolver.GetPower(hand[i], vira);
                if (power < bestPower)
                {
                    bestPower = power;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static int ChooseStrongest(IReadOnlyList<Card> hand, Card vira)
        {
            int bestIndex = 0;
            int bestPower = CardValueResolver.GetPower(hand[0], vira);

            for (int i = 1; i < hand.Count; i++)
            {
                int power = CardValueResolver.GetPower(hand[i], vira);
                if (power > bestPower)
                {
                    bestPower = power;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static int ChooseWeakestWinningCard(IReadOnlyList<Card> hand, Card cardToBeat, Card vira)
        {
            int chosenIndex = -1;
            int chosenPower = int.MaxValue;

            for (int i = 0; i < hand.Count; i++)
            {
                if (CardValueResolver.Compare(hand[i], cardToBeat, vira) <= 0)
                {
                    continue;
                }

                int power = CardValueResolver.GetPower(hand[i], vira);
                if (power < chosenPower)
                {
                    chosenPower = power;
                    chosenIndex = i;
                }
            }

            return chosenIndex;
        }

        private static bool TryChooseTrueSignal(IReadOnlyList<Card> hand, Card vira, out SignalType signal)
        {
            int threeCount = 0;
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Rank == Rank.Three)
                {
                    threeCount++;
                }
            }

            if (threeCount >= 2)
            {
                signal = SignalType.BothShouldersDoubleThree;
                return true;
            }

            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];
                if (CardValueResolver.IsManilha(card, vira))
                {
                    signal = GetManilhaSignal(card.Suit);
                    return true;
                }
            }

            for (int i = 0; i < hand.Count; i++)
            {
                switch (hand[i].Rank)
                {
                    case Rank.Three:
                        signal = SignalType.OneShoulderThree;
                        return true;
                    case Rank.Ace:
                        signal = SignalType.NoseTouchAce;
                        return true;
                    case Rank.King:
                        signal = SignalType.ChinHandKing;
                        return true;
                    case Rank.Queen:
                        signal = SignalType.EarHandQueen;
                        return true;
                    case Rank.Jack:
                        signal = SignalType.EarToChinJack;
                        return true;
                }
            }

            signal = SignalType.WinkZap;
            return false;
        }

        private static SignalType GetManilhaSignal(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return SignalType.WinkZap;
                case Suit.Hearts:
                    return SignalType.RaiseEyebrowHearts;
                case Suit.Spades:
                    return SignalType.PuffCheekSpades;
                case Suit.Diamonds:
                    return SignalType.TongueOrNoseDiamonds;
                default:
                    return SignalType.WinkZap;
            }
        }

        private static SignalType GetRandomSignal()
        {
            SignalType[] signals = (SignalType[])System.Enum.GetValues(typeof(SignalType));
            return signals[Random.Range(0, signals.Length)];
        }
    }
}

