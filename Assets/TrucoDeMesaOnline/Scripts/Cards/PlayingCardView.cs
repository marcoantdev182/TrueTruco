using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class PlayingCardView : MonoBehaviour
    {
        private Renderer bodyRenderer;
        private TextMesh label;

        public Card Card { get; private set; }
        public bool IsFaceUp { get; private set; }

        public void Initialize(Card card, bool faceUp)
        {
            Card = card;
            IsFaceUp = faceUp;
            EnsureVisuals();

            bodyRenderer.sharedMaterial = RuntimeMaterialFactory.GetMaterial(
                faceUp ? "CardFace" : "CardBack",
                faceUp ? new Color(0.95f, 0.92f, 0.82f) : new Color(0.15f, 0.25f, 0.48f));

            label.text = faceUp ? card.ShortName : "";
            label.color = GetTextColor(card.Suit);
        }

        private void EnsureVisuals()
        {
            if (bodyRenderer == null)
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
                body.name = "Card Body";
                body.transform.SetParent(transform, false);
                body.transform.localScale = new Vector3(0.52f, 0.025f, 0.76f);
                bodyRenderer = body.GetComponent<Renderer>();
            }

            if (label == null)
            {
                GameObject labelObject = new GameObject("Card Label");
                labelObject.transform.SetParent(transform, false);
                labelObject.transform.localPosition = new Vector3(0f, 0.03f, 0f);
                labelObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                labelObject.transform.localScale = Vector3.one * 0.1f;

                label = labelObject.AddComponent<TextMesh>();
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.characterSize = 1.2f;
                label.fontSize = 64;
            }
        }

        private static Color GetTextColor(Suit suit)
        {
            if (suit == Suit.Hearts || suit == Suit.Diamonds)
            {
                return new Color(0.75f, 0.05f, 0.05f);
            }

            return new Color(0.05f, 0.05f, 0.05f);
        }
    }
}
