using UnityEngine;

namespace TrucoDeMesaOnline
{
    [DefaultExecutionOrder(-1000)]
    public sealed class LocalOfflineSceneRoot : MonoBehaviour
    {
        [SerializeField] private bool autoStart = true;

        private void Awake()
        {
            if (!autoStart)
            {
                return;
            }

            EnsureGameManager();
        }

        private static void EnsureGameManager()
        {
            GameManager existing = FindObjectOfType<GameManager>();
            if (existing != null)
            {
                return;
            }

            GameObject root = new GameObject("Truco de Mesa Online - Local MVP");
            GameManager manager = root.AddComponent<GameManager>();
            manager.ConfigureForLocalOffline();
        }
    }
}

