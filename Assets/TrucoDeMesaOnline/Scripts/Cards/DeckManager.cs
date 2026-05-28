using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class DeckManager : MonoBehaviour
    {
        [SerializeField] private int fixedSeed;

        private readonly List<Card> deck = new List<Card>();
        private System.Random random;

        public int RemainingCount
        {
            get { return deck.Count; }
        }

        public void ResetAndShuffle()
        {
            ResetDeck();
            Shuffle();
        }

        public void ResetDeck()
        {
            deck.Clear();

            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    deck.Add(new Card(rank, suit));
                }
            }
        }

        public void Shuffle()
        {
            int seed = fixedSeed == 0 ? Environment.TickCount : fixedSeed;
            random = new System.Random(seed);

            for (int i = deck.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                Card temp = deck[i];
                deck[i] = deck[swapIndex];
                deck[swapIndex] = temp;
            }
        }

        public Card Draw()
        {
            if (deck.Count == 0)
            {
                throw new InvalidOperationException("Deck is empty.");
            }

            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        public Dictionary<SeatId, List<Card>> DealHands()
        {
            Dictionary<SeatId, List<Card>> hands = new Dictionary<SeatId, List<Card>>();
            SeatId[] seats =
            {
                SeatId.LocalPlayer,
                SeatId.RightRival,
                SeatId.Partner,
                SeatId.LeftRival
            };

            for (int i = 0; i < seats.Length; i++)
            {
                hands[seats[i]] = new List<Card>(GameConstants.CardsPerPlayer);
            }

            for (int cardIndex = 0; cardIndex < GameConstants.CardsPerPlayer; cardIndex++)
            {
                for (int seatIndex = 0; seatIndex < seats.Length; seatIndex++)
                {
                    hands[seats[seatIndex]].Add(Draw());
                }
            }

            return hands;
        }
    }
}

