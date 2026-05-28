using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameMode gameMode = GameMode.LocalOffline;
        [SerializeField] private bool autoStart = true;

        private SeatManager seatManager;
        private TableManager tableManager;
        private PlayerController playerController;
        private DeckManager deckManager;
        private HandManager handManager;
        private RoundManager roundManager;
        private TurnManager turnManager;
        private ScoreManager scoreManager;
        private SignalManager signalManager;
        private UIManager uiManager;
        private NetworkManagerController networkManagerController;
        private LobbyManager lobbyManager;

        private bool eventsWired;
        private bool matchStarted;
        private Coroutine botTurnRoutine;
        private Coroutine delayedRoundRoutine;

        public void ConfigureForLocalOffline()
        {
            gameMode = GameMode.LocalOffline;
            autoStart = true;
        }

        private void Awake()
        {
            EnsureManagers();
        }

        private void Start()
        {
            if (autoStart && gameMode == GameMode.LocalOffline)
            {
                StartLocalGame();
            }
        }

        public void StartLocalGame()
        {
            EnsureManagers();
            BuildLocalScene();
            WireEvents();

            if (!matchStarted)
            {
                scoreManager.ResetMatch();
                matchStarted = true;
            }

            StartNewRound();
        }

        private void EnsureManagers()
        {
            seatManager = GetOrCreateChildManager<SeatManager>("Seat Manager");
            tableManager = GetOrCreateChildManager<TableManager>("Table Manager");
            playerController = GetOrCreateChildManager<PlayerController>("Player Controller");
            deckManager = GetOrCreateChildManager<DeckManager>("Deck Manager");
            handManager = GetOrCreateChildManager<HandManager>("Hand Manager");
            roundManager = GetOrCreateChildManager<RoundManager>("Round Manager");
            turnManager = GetOrCreateChildManager<TurnManager>("Turn Manager");
            scoreManager = GetOrCreateChildManager<ScoreManager>("Score Manager");
            signalManager = GetOrCreateChildManager<SignalManager>("Signal Manager");
            uiManager = GetOrCreateChildManager<UIManager>("UI Manager");
            networkManagerController = GetOrCreateChildManager<NetworkManagerController>("Network Manager Controller");
            lobbyManager = GetOrCreateChildManager<LobbyManager>("Lobby Manager");
        }

        private T GetOrCreateChildManager<T>(string childName) where T : Component
        {
            T existing = GetComponentInChildren<T>();
            if (existing != null)
            {
                return existing;
            }

            GameObject child = new GameObject(childName);
            child.transform.SetParent(transform, false);
            return child.AddComponent<T>();
        }

        private void BuildLocalScene()
        {
            seatManager.BuildSeats(transform);
            tableManager.BuildLocalTable(seatManager);

            Camera camera = GetOrCreatePlayerCamera();
            playerController.InitializeLocalPlayer(seatManager.GetSeat(SeatId.LocalPlayer), camera);
            uiManager.BuildHud();
        }

        private Camera GetOrCreatePlayerCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Local Player Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.fieldOfView = 68f;
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 100f;

            if (camera.GetComponent<AudioListener>() == null && FindObjectOfType<AudioListener>() == null)
            {
                camera.gameObject.AddComponent<AudioListener>();
            }

            return camera;
        }

        private void WireEvents()
        {
            if (eventsWired)
            {
                return;
            }

            uiManager.CardClicked += handManager.RequestPlayCard;
            uiManager.SignalClicked += OnSignalClicked;
            uiManager.TrucoClicked += OnTrucoClicked;
            uiManager.RunClicked += OnRunClicked;

            handManager.LocalCardRequested += OnLocalCardRequested;
            handManager.LocalHandChanged += OnLocalHandChanged;
            turnManager.TurnChanged += OnTurnChanged;
            scoreManager.ScoresChanged += OnScoresChanged;
            signalManager.SignalSent += OnSignalSent;

            eventsWired = true;
        }

        private void StartNewRound()
        {
            StopBotTurn();
            StopDelayedRound();

            tableManager.ClearAllCards();
            deckManager.ResetAndShuffle();

            Dictionary<SeatId, List<Card>> hands = deckManager.DealHands();
            Card vira = deckManager.Draw();

            scoreManager.ResetRoundValue();
            roundManager.StartRound(hands, vira);
            tableManager.ShowVira(vira);
            uiManager.SetVira(vira);

            handManager.SetLocalHand(roundManager.GetHand(SeatId.LocalPlayer));
            turnManager.SetCurrentSeat(SeatId.LocalPlayer);

            uiManager.SetStatus("Nova rodada. Sua vez de abrir.");
            RefreshUiForCurrentTurn();
        }

        private void OnLocalCardRequested(int handIndex, Card requestedCard)
        {
            if (!roundManager.IsRoundActive)
            {
                uiManager.SetStatus("Rodada encerrada.");
                return;
            }

            if (!turnManager.IsLocalTurn)
            {
                uiManager.SetStatus("Aguarde: turno de " + SeatUtility.GetDisplayName(turnManager.CurrentSeat) + ".");
                return;
            }

            TryPlayCard(SeatId.LocalPlayer, handIndex);
        }

        private void TryPlayCard(SeatId seat, int handIndex)
        {
            if (turnManager.CurrentSeat != seat)
            {
                return;
            }

            if (!roundManager.TryPlayCard(seat, handIndex, out PlayedCard playedCard))
            {
                return;
            }

            tableManager.PlacePlayedCard(playedCard);
            Debug.Log("[Card] " + SeatUtility.GetDisplayName(seat) + " jogou " + playedCard.Card.DisplayName + ".");

            if (seat == SeatId.LocalPlayer)
            {
                handManager.SetLocalHand(roundManager.GetHand(SeatId.LocalPlayer));
            }

            if (roundManager.CurrentTrickCount >= GameConstants.PlayerCount)
            {
                StopBotTurn();
                StartCoroutine(ResolveTrickRoutine());
                return;
            }

            turnManager.Advance();
            ContinueIfBotTurn();
        }

        private IEnumerator ResolveTrickRoutine()
        {
            uiManager.SetHandInteractable(false);
            uiManager.SetStatus("Mesa fechada. Resolvendo a vaza...");
            yield return new WaitForSeconds(0.9f);

            TrickResult result = roundManager.ResolveCurrentTrick();
            uiManager.SetStatus(
                SeatUtility.GetDisplayName(result.WinningSeat) +
                " venceu a vaza. Placar de vazas: " +
                result.LocalTeamTricks +
                " x " +
                result.RivalTeamTricks +
                ".");

            if (roundManager.TryGetRoundWinner(out TeamId roundWinner))
            {
                yield return new WaitForSeconds(1f);
                scoreManager.AwardRound(roundWinner);

                if (scoreManager.HasMatchWinner(out TeamId matchWinner))
                {
                    uiManager.SetStatus(ScoreManager.GetTeamLabel(matchWinner) + " venceu a partida. Reiniciando placar...");
                    yield return new WaitForSeconds(2.2f);
                    scoreManager.ResetMatch();
                }
                else
                {
                    uiManager.SetStatus(ScoreManager.GetTeamLabel(roundWinner) + " levou a rodada.");
                    yield return new WaitForSeconds(1.6f);
                }

                StartNewRound();
                yield break;
            }

            yield return new WaitForSeconds(1f);
            tableManager.ClearPlayedCards();
            turnManager.SetCurrentSeat(result.WinningSeat);
            RefreshUiForCurrentTurn();
            ContinueIfBotTurn();
        }

        private void ContinueIfBotTurn()
        {
            RefreshUiForCurrentTurn();

            if (!roundManager.IsRoundActive || turnManager.CurrentSeat == SeatId.LocalPlayer)
            {
                return;
            }

            StopBotTurn();
            botTurnRoutine = StartCoroutine(BotTurnRoutine(turnManager.CurrentSeat));
        }

        private IEnumerator BotTurnRoutine(SeatId botSeat)
        {
            uiManager.SetStatus(SeatUtility.GetDisplayName(botSeat) + " pensando...");
            yield return new WaitForSeconds(0.25f);

            TrySendBotSignal(botSeat);

            yield return new WaitForSeconds(GameConstants.BotPlayDelay);

            if (roundManager.IsRoundActive && turnManager.CurrentSeat == botSeat)
            {
                TryPlayCard(botSeat, 0);
            }
        }

        private void TrySendBotSignal(SeatId botSeat)
        {
            if (!roundManager.IsRoundActive || botSeat == SeatId.LocalPlayer || UnityEngine.Random.value > 0.72f)
            {
                return;
            }

            SignalType signal = ChooseBotSignal(botSeat);
            signalManager.SendSignal(botSeat, signal);
        }

        private SignalType ChooseBotSignal(SeatId botSeat)
        {
            IReadOnlyList<Card> botHand = roundManager.GetHand(botSeat);
            if (UnityEngine.Random.value <= 0.62f && TryChooseTrueSignal(botHand, out SignalType trueSignal))
            {
                return trueSignal;
            }

            return GetRandomSignal();
        }

        private bool TryChooseTrueSignal(IReadOnlyList<Card> hand, out SignalType signal)
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
                if (CardValueResolver.IsManilha(card, roundManager.Vira))
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

        private SignalType GetManilhaSignal(Suit suit)
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

        private SignalType GetRandomSignal()
        {
            SignalType[] signals = (SignalType[])System.Enum.GetValues(typeof(SignalType));
            return signals[UnityEngine.Random.Range(0, signals.Length)];
        }

        private void OnSignalClicked(SignalType signal)
        {
            signalManager.SendSignal(SeatId.LocalPlayer, signal);
            uiManager.SetStatus("Sinal enviado: " + SignalDefinition.GetLabel(signal) + ".");
        }

        private void OnSignalSent(SignalEvent signalEvent)
        {
            tableManager.PlayGesture(signalEvent.SourceSeat, signalEvent.Signal);

            if (signalEvent.SourceSeat == SeatId.LocalPlayer)
            {
                return;
            }

            if (tableManager.IsSeatInCameraView(signalEvent.SourceSeat, playerController.PlayerCamera, 34f))
            {
                uiManager.SetStatus(
                    "Voce viu " +
                    SeatUtility.GetDisplayName(signalEvent.SourceSeat) +
                    " fazer: " +
                    SignalDefinition.GetLabel(signalEvent.Signal) +
                    ".");
            }
            else
            {
                Debug.Log("[Signal] Fora do seu campo de visao: " + SeatUtility.GetDisplayName(signalEvent.SourceSeat));
            }
        }

        private void OnTrucoClicked()
        {
            int value = scoreManager.RequestTruco(SeatId.LocalPlayer);
            uiManager.SetStatus("Voce pediu Truco. Rodada agora vale " + value + ".");
        }

        private void OnRunClicked()
        {
            if (!roundManager.IsRoundActive)
            {
                return;
            }

            StopBotTurn();
            roundManager.EndRound();
            TeamId winner = scoreManager.Run(SeatId.LocalPlayer);
            uiManager.SetStatus("Voce correu. Ponto para " + ScoreManager.GetTeamLabel(winner) + ".");
            delayedRoundRoutine = StartCoroutine(StartRoundAfterRunRoutine());
        }

        private IEnumerator StartRoundAfterRunRoutine()
        {
            yield return new WaitForSeconds(1.2f);

            if (scoreManager.HasMatchWinner(out TeamId matchWinner))
            {
                uiManager.SetStatus(ScoreManager.GetTeamLabel(matchWinner) + " venceu a partida. Reiniciando placar...");
                yield return new WaitForSeconds(2.2f);
                scoreManager.ResetMatch();
            }

            StartNewRound();
        }

        private void OnLocalHandChanged(IReadOnlyList<Card> hand)
        {
            bool canPlay = roundManager.IsRoundActive && turnManager.IsLocalTurn;
            uiManager.RefreshHand(hand, canPlay);
        }

        private void OnTurnChanged(SeatId currentSeat)
        {
            uiManager.SetTurn(currentSeat);
            uiManager.SetHandInteractable(roundManager.IsRoundActive && currentSeat == SeatId.LocalPlayer);
        }

        private void OnScoresChanged()
        {
            uiManager.SetScore(scoreManager.LocalTeamScore, scoreManager.RivalTeamScore, scoreManager.CurrentRoundValue);
        }

        private void RefreshUiForCurrentTurn()
        {
            uiManager.SetTurn(turnManager.CurrentSeat);
            uiManager.SetScore(scoreManager.LocalTeamScore, scoreManager.RivalTeamScore, scoreManager.CurrentRoundValue);
            uiManager.SetVira(roundManager.Vira);
            uiManager.SetHandInteractable(roundManager.IsRoundActive && turnManager.IsLocalTurn);
        }

        private void StopBotTurn()
        {
            if (botTurnRoutine != null)
            {
                StopCoroutine(botTurnRoutine);
                botTurnRoutine = null;
            }
        }

        private void StopDelayedRound()
        {
            if (delayedRoundRoutine != null)
            {
                StopCoroutine(delayedRoundRoutine);
                delayedRoundRoutine = null;
            }
        }
    }
}
