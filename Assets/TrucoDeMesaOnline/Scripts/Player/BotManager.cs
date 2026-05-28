using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class BotManager : MonoBehaviour
    {
        private readonly Dictionary<SeatId, BotPlayerController> botsBySeat = new Dictionary<SeatId, BotPlayerController>();

        public void BuildLocalBots()
        {
            ClearBots();
            CreateBot(SeatId.Partner, BotPersonality.Partner);
            CreateBot(SeatId.RightRival, BotPersonality.BoldRival);
            CreateBot(SeatId.LeftRival, BotPersonality.CautiousRival);
        }

        public bool TryGetBot(SeatId seat, out BotPlayerController bot)
        {
            return botsBySeat.TryGetValue(seat, out bot);
        }

        public bool IsBotSeat(SeatId seat)
        {
            return botsBySeat.ContainsKey(seat);
        }

        private void CreateBot(SeatId seat, BotPersonality personality)
        {
            GameObject botObject = new GameObject("Bot - " + SeatUtility.GetDisplayName(seat));
            botObject.transform.SetParent(transform, false);

            BotPlayerController bot = botObject.AddComponent<BotPlayerController>();
            bot.Configure(seat, personality);
            botsBySeat.Add(seat, bot);
        }

        private void ClearBots()
        {
            foreach (KeyValuePair<SeatId, BotPlayerController> entry in botsBySeat)
            {
                if (entry.Value != null)
                {
                    DestroyObject(entry.Value.gameObject);
                }
            }

            botsBySeat.Clear();
        }

        private static void DestroyObject(GameObject target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
    }
}

