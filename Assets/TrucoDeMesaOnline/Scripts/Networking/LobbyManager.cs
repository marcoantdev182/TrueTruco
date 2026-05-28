using System;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class LobbyManager : MonoBehaviour
    {
        public event Action<string> PrivateCodeCreated;
        public event Action<string> PrivateCodeJoined;

        public string CurrentPrivateCode { get; private set; }

        public string CreatePrivateLobby()
        {
            CurrentPrivateCode = GenerateLocalPlaceholderCode();
            Debug.Log("[Lobby] Placeholder lobby created with code: " + CurrentPrivateCode);
            PrivateCodeCreated?.Invoke(CurrentPrivateCode);
            return CurrentPrivateCode;
        }

        public void JoinPrivateLobby(string privateCode)
        {
            CurrentPrivateCode = privateCode;
            Debug.Log("[Lobby] Placeholder join with code: " + CurrentPrivateCode);
            PrivateCodeJoined?.Invoke(CurrentPrivateCode);
        }

        public void LeaveLobby()
        {
            Debug.Log("[Lobby] Placeholder leave lobby.");
            CurrentPrivateCode = string.Empty;
        }

        private static string GenerateLocalPlaceholderCode()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            System.Random random = new System.Random(Environment.TickCount);
            char[] code = new char[6];

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = alphabet[random.Next(alphabet.Length)];
            }

            return new string(code);
        }
    }
}

