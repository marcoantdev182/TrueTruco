using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class HandManager : MonoBehaviour
    {
        private readonly List<Card> localHand = new List<Card>();

        public event Action<IReadOnlyList<Card>> LocalHandChanged;
        public event Action<int, Card> LocalCardRequested;

        public IReadOnlyList<Card> LocalHand
        {
            get { return localHand; }
        }

        public void SetLocalHand(IReadOnlyList<Card> cards)
        {
            localHand.Clear();

            for (int i = 0; i < cards.Count; i++)
            {
                localHand.Add(cards[i]);
            }

            LocalHandChanged?.Invoke(localHand);
        }

        public void RequestPlayCard(int handIndex)
        {
            if (handIndex < 0 || handIndex >= localHand.Count)
            {
                Debug.LogWarning("Invalid local card index: " + handIndex);
                return;
            }

            LocalCardRequested?.Invoke(handIndex, localHand[handIndex]);
        }
    }
}

