using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrucoDeMesaOnline
{
    public sealed class UIManager : MonoBehaviour
    {
        public event Action<int> CardClicked;
        public event Action<SignalType> SignalClicked;
        public event Action TrucoClicked;
        public event Action RunClicked;

        private Canvas canvas;
        private RectTransform handPanel;
        private Text scoreText;
        private Text turnText;
        private Text statusText;
        private Text viraText;
        private readonly List<Button> cardButtons = new List<Button>();
        private Font cachedFont;

        public void BuildHud()
        {
            ClearExistingHud();
            EnsureEventSystem();

            GameObject canvasObject = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            BuildTopBar(canvasObject.transform);
            BuildHandPanel(canvasObject.transform);
            BuildActionPanel(canvasObject.transform);
            BuildSignalPanel(canvasObject.transform);
        }

        public void SetScore(int localScore, int rivalScore, int roundValue)
        {
            if (scoreText != null)
            {
                scoreText.text = "Nos " + localScore + " x " + rivalScore + " Eles   |   Vale " + roundValue;
            }
        }

        public void SetTurn(SeatId currentSeat)
        {
            if (turnText != null)
            {
                turnText.text = "Turno: " + SeatUtility.GetDisplayName(currentSeat);
            }
        }

        public void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        public void SetVira(Card vira)
        {
            if (viraText != null)
            {
                viraText.text = "Vira: " + vira.ShortName + "   Manilha: " + Card.GetRankName(CardValueResolver.GetManilhaRank(vira));
            }
        }

        public void RefreshHand(IReadOnlyList<Card> hand, bool canPlay)
        {
            if (handPanel == null)
            {
                return;
            }

            ClearChildren(handPanel);
            cardButtons.Clear();

            for (int i = 0; i < hand.Count; i++)
            {
                int capturedIndex = i;
                Button button = CreateButton(handPanel, hand[i].ShortName + "\n" + hand[i].DisplayName, new Vector2(150f, 88f));
                button.interactable = canPlay;
                button.onClick.AddListener(() => CardClicked?.Invoke(capturedIndex));
                cardButtons.Add(button);
            }
        }

        public void SetHandInteractable(bool canPlay)
        {
            for (int i = 0; i < cardButtons.Count; i++)
            {
                cardButtons[i].interactable = canPlay;
            }
        }

        private void BuildTopBar(Transform parent)
        {
            RectTransform topBar = CreatePanel(parent, "Top Bar", new Color(0.08f, 0.09f, 0.10f, 0.82f));
            topBar.anchorMin = new Vector2(0f, 1f);
            topBar.anchorMax = new Vector2(1f, 1f);
            topBar.pivot = new Vector2(0.5f, 1f);
            topBar.anchoredPosition = Vector2.zero;
            topBar.sizeDelta = new Vector2(0f, 76f);

            HorizontalLayoutGroup layout = topBar.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.padding = new RectOffset(18, 18, 10, 10);
            layout.spacing = 24f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            scoreText = CreateText(topBar, "Nos 0 x 0 Eles   |   Vale 1", 24, TextAnchor.MiddleLeft);
            AddLayout(scoreText.gameObject, 360f, 56f);

            turnText = CreateText(topBar, "Turno: Voce", 24, TextAnchor.MiddleLeft);
            AddLayout(turnText.gameObject, 280f, 56f);

            viraText = CreateText(topBar, "Vira: -", 22, TextAnchor.MiddleLeft);
            AddLayout(viraText.gameObject, 360f, 56f);

            statusText = CreateText(topBar, "Clique em uma carta para jogar.", 22, TextAnchor.MiddleLeft);
            AddLayout(statusText.gameObject, 760f, 56f);
        }

        private void BuildHandPanel(Transform parent)
        {
            handPanel = CreatePanel(parent, "Hand Panel", new Color(0.05f, 0.05f, 0.05f, 0.72f));
            handPanel.anchorMin = new Vector2(0.5f, 0f);
            handPanel.anchorMax = new Vector2(0.5f, 0f);
            handPanel.pivot = new Vector2(0.5f, 0f);
            handPanel.anchoredPosition = new Vector2(0f, 22f);
            handPanel.sizeDelta = new Vector2(560f, 118f);

            HorizontalLayoutGroup layout = handPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.padding = new RectOffset(14, 14, 12, 12);
            layout.spacing = 14f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        private void BuildActionPanel(Transform parent)
        {
            RectTransform actionPanel = CreatePanel(parent, "Action Panel", new Color(0.05f, 0.05f, 0.05f, 0.72f));
            actionPanel.anchorMin = new Vector2(0f, 0f);
            actionPanel.anchorMax = new Vector2(0f, 0f);
            actionPanel.pivot = new Vector2(0f, 0f);
            actionPanel.anchoredPosition = new Vector2(18f, 22f);
            actionPanel.sizeDelta = new Vector2(230f, 118f);

            HorizontalLayoutGroup layout = actionPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 10f;

            Button truco = CreateButton(actionPanel, "TRUCO", new Vector2(96f, 72f));
            truco.onClick.AddListener(() => TrucoClicked?.Invoke());

            Button run = CreateButton(actionPanel, "CORRER", new Vector2(96f, 72f));
            run.onClick.AddListener(() => RunClicked?.Invoke());
        }

        private void BuildSignalPanel(Transform parent)
        {
            RectTransform signalPanel = CreatePanel(parent, "Signal Panel", new Color(0.05f, 0.05f, 0.05f, 0.72f));
            signalPanel.anchorMin = new Vector2(1f, 0.5f);
            signalPanel.anchorMax = new Vector2(1f, 0.5f);
            signalPanel.pivot = new Vector2(1f, 0.5f);
            signalPanel.anchoredPosition = new Vector2(-18f, -10f);
            signalPanel.sizeDelta = new Vector2(294f, 520f);

            VerticalLayoutGroup layout = signalPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 8f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Text title = CreateText(signalPanel, "Sinais", 24, TextAnchor.MiddleCenter);
            AddLayout(title.gameObject, 260f, 34f);

            foreach (SignalType signal in Enum.GetValues(typeof(SignalType)))
            {
                SignalType capturedSignal = signal;
                Button button = CreateButton(signalPanel, SignalDefinition.GetLabel(signal), new Vector2(260f, 34f));
                button.onClick.AddListener(() => SignalClicked?.Invoke(capturedSignal));
            }
        }

        private RectTransform CreatePanel(Transform parent, string panelName, Color color)
        {
            GameObject panelObject = new GameObject(panelName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObject.transform.SetParent(parent, false);
            Image image = panelObject.GetComponent<Image>();
            image.color = color;
            return panelObject.GetComponent<RectTransform>();
        }

        private Button CreateButton(Transform parent, string label, Vector2 preferredSize)
        {
            GameObject buttonObject = new GameObject(label + " Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.72f, 0.65f, 0.48f, 0.96f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.72f, 0.65f, 0.48f, 0.96f);
            colors.highlightedColor = new Color(0.86f, 0.78f, 0.58f, 1f);
            colors.pressedColor = new Color(0.54f, 0.48f, 0.35f, 1f);
            colors.disabledColor = new Color(0.28f, 0.28f, 0.28f, 0.65f);
            button.colors = colors;

            Text text = CreateText(buttonObject.transform, label, 17, TextAnchor.MiddleCenter);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);
            text.color = new Color(0.08f, 0.07f, 0.05f);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 11;
            text.resizeTextMaxSize = 18;

            AddLayout(buttonObject, preferredSize.x, preferredSize.y);
            return button;
        }

        private Text CreateText(Transform parent, string value, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = GetDefaultFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private void AddLayout(GameObject target, float preferredWidth, float preferredHeight)
        {
            LayoutElement layout = target.GetComponent<LayoutElement>();
            if (layout == null)
            {
                layout = target.AddComponent<LayoutElement>();
            }

            layout.preferredWidth = preferredWidth;
            layout.preferredHeight = preferredHeight;
            layout.minWidth = preferredWidth;
            layout.minHeight = preferredHeight;
        }

        private Font GetDefaultFont()
        {
            if (cachedFont != null)
            {
                return cachedFont;
            }

            cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (cachedFont == null)
            {
                cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return cachedFont;
        }

        private void ClearExistingHud()
        {
            if (canvas != null)
            {
                DestroyObject(canvas.gameObject);
                canvas = null;
            }

            cardButtons.Clear();
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyObject(parent.GetChild(i).gameObject);
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.position = Vector3.zero;
        }

        private static void DestroyObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

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
