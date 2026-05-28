using System;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class SignalManager : MonoBehaviour
    {
        public event Action<SignalEvent> SignalSent;

        public void SendSignal(SeatId sourceSeat, SignalType signal)
        {
            SignalEvent signalEvent = new SignalEvent(sourceSeat, signal, Time.time);
            Debug.Log("[Signal] " + SeatUtility.GetDisplayName(sourceSeat) + ": " + SignalDefinition.GetConsoleText(signal));
            SignalSent?.Invoke(signalEvent);
        }
    }
}

