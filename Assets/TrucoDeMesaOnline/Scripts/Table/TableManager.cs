using System.Collections.Generic;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class TableManager : MonoBehaviour
    {
        private readonly Dictionary<SeatId, GestureController> avatarGestures = new Dictionary<SeatId, GestureController>();
        private readonly List<PlayingCardView> playedCardViews = new List<PlayingCardView>();

        private Transform generatedRoot;
        private Transform cardsRoot;
        private PlayingCardView viraView;

        public void BuildLocalTable(SeatManager seatManager)
        {
            ClearGeneratedObjects();

            GameObject root = new GameObject("Generated Local Table");
            root.transform.SetParent(transform, false);
            generatedRoot = root.transform;

            cardsRoot = new GameObject("Cards On Table").transform;
            cardsRoot.SetParent(generatedRoot, false);

            CreateLighting();
            CreateEnvironment();
            CreateTable();
            CreateSeatsAndAvatars(seatManager);
        }

        public void ShowVira(Card vira)
        {
            if (viraView != null)
            {
                DestroyObject(viraView.gameObject);
            }

            GameObject cardObject = new GameObject("Vira - " + vira.ShortName);
            cardObject.transform.SetParent(cardsRoot, false);
            cardObject.transform.position = new Vector3(0.88f, GameConstants.TableHeight + 0.09f, -0.72f);
            cardObject.transform.rotation = Quaternion.Euler(0f, -28f, 0f);

            viraView = cardObject.AddComponent<PlayingCardView>();
            viraView.Initialize(vira, true);
        }

        public void PlacePlayedCard(PlayedCard playedCard)
        {
            GameObject cardObject = new GameObject("Played " + playedCard.Card.ShortName + " - " + SeatUtility.GetShortName(playedCard.Seat));
            cardObject.transform.SetParent(cardsRoot, false);
            cardObject.transform.position = GetPlayedCardPosition(playedCard.Seat);
            cardObject.transform.rotation = Quaternion.Euler(0f, GetPlayedCardYaw(playedCard.Seat), 0f);

            PlayingCardView view = cardObject.AddComponent<PlayingCardView>();
            view.Initialize(playedCard.Card, true);
            playedCardViews.Add(view);
        }

        public void ClearPlayedCards()
        {
            for (int i = 0; i < playedCardViews.Count; i++)
            {
                if (playedCardViews[i] != null)
                {
                    DestroyObject(playedCardViews[i].gameObject);
                }
            }

            playedCardViews.Clear();
        }

        public void ClearAllCards()
        {
            ClearPlayedCards();

            if (viraView != null)
            {
                DestroyObject(viraView.gameObject);
                viraView = null;
            }
        }

        public void PlayGesture(SeatId sourceSeat, SignalType signal)
        {
            if (avatarGestures.TryGetValue(sourceSeat, out GestureController gesture))
            {
                gesture.PlayGesture(signal);
            }
        }

        public bool IsSeatInCameraView(SeatId seat, Camera camera, float maxAngle)
        {
            if (camera == null || !avatarGestures.TryGetValue(seat, out GestureController gesture))
            {
                return false;
            }

            Vector3 focusPosition = gesture.FocusPosition;
            Vector3 direction = focusPosition - camera.transform.position;
            if (direction.sqrMagnitude <= 0.001f)
            {
                return false;
            }

            float angle = Vector3.Angle(camera.transform.forward, direction.normalized);
            if (angle > maxAngle)
            {
                return false;
            }

            Vector3 viewportPoint = camera.WorldToViewportPoint(focusPosition);
            return viewportPoint.z > 0f &&
                   viewportPoint.x > 0.05f &&
                   viewportPoint.x < 0.95f &&
                   viewportPoint.y > 0.08f &&
                   viewportPoint.y < 0.94f;
        }

        private void CreateLighting()
        {
            GameObject lightObject = new GameObject("Warm Table Light");
            lightObject.transform.SetParent(generatedRoot, false);
            lightObject.transform.position = new Vector3(0f, 4.3f, -0.8f);

            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 8f;
            light.intensity = 2.1f;
            light.color = new Color(1f, 0.82f, 0.58f);
        }

        private void CreateEnvironment()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Simple Bar Floor";
            floor.transform.SetParent(generatedRoot, false);
            floor.transform.position = new Vector3(0f, -0.04f, 0f);
            floor.transform.localScale = new Vector3(7.5f, 0.08f, 7.5f);
            floor.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("FloorWarmGray", new Color(0.33f, 0.31f, 0.28f));

            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "Back Bar Wall";
            backWall.transform.SetParent(generatedRoot, false);
            backWall.transform.position = new Vector3(0f, 1.5f, 3.8f);
            backWall.transform.localScale = new Vector3(7.5f, 3f, 0.08f);
            backWall.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("WallMutedGreen", new Color(0.24f, 0.34f, 0.29f));

            GameObject sideCounter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sideCounter.name = "Placeholder Bar Counter";
            sideCounter.transform.SetParent(generatedRoot, false);
            sideCounter.transform.position = new Vector3(-3.05f, 0.65f, 1.8f);
            sideCounter.transform.localScale = new Vector3(0.55f, 1.3f, 2.1f);
            sideCounter.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("DarkWood", new Color(0.20f, 0.10f, 0.04f));
        }

        private void CreateTable()
        {
            GameObject tabletop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tabletop.name = "Round Wooden Table";
            tabletop.transform.SetParent(generatedRoot, false);
            tabletop.transform.position = new Vector3(0f, GameConstants.TableHeight, 0f);
            tabletop.transform.localScale = new Vector3(GameConstants.TableRadius, 0.08f, GameConstants.TableRadius);
            tabletop.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("TableWood", new Color(0.42f, 0.22f, 0.09f));

            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Table Pedestal";
            pedestal.transform.SetParent(generatedRoot, false);
            pedestal.transform.position = new Vector3(0f, 0.38f, 0f);
            pedestal.transform.localScale = new Vector3(0.18f, 0.38f, 0.18f);
            pedestal.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("TableWood", new Color(0.42f, 0.22f, 0.09f));
        }

        private void CreateSeatsAndAvatars(SeatManager seatManager)
        {
            avatarGestures.Clear();

            for (int i = 0; i < seatManager.Seats.Count; i++)
            {
                Seat seat = seatManager.Seats[i];
                CreateChair(seat);

                if (seat.SeatId != SeatId.LocalPlayer)
                {
                    GestureController gesture = CreateAvatar(seat);
                    avatarGestures.Add(seat.SeatId, gesture);
                }

                CreateSeatLabel(seat);
            }
        }

        private void CreateChair(Seat seat)
        {
            GameObject chair = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chair.name = SeatUtility.GetDisplayName(seat.SeatId) + " Chair";
            chair.transform.SetParent(generatedRoot, false);
            chair.transform.position = seat.transform.position + Vector3.up * 0.32f - seat.transform.forward * 0.16f;
            chair.transform.rotation = seat.transform.rotation;
            chair.transform.localScale = new Vector3(0.72f, 0.18f, 0.72f);
            chair.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("ChairWood", new Color(0.26f, 0.14f, 0.06f));

            GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
            back.name = SeatUtility.GetDisplayName(seat.SeatId) + " Chair Back";
            back.transform.SetParent(generatedRoot, false);
            back.transform.position = seat.transform.position + Vector3.up * 0.82f - seat.transform.forward * 0.52f;
            back.transform.rotation = seat.transform.rotation;
            back.transform.localScale = new Vector3(0.75f, 0.9f, 0.12f);
            back.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("ChairWood", new Color(0.26f, 0.14f, 0.06f));
        }

        private GestureController CreateAvatar(Seat seat)
        {
            GameObject avatar = new GameObject(SeatUtility.GetDisplayName(seat.SeatId) + " Avatar");
            avatar.transform.SetParent(generatedRoot, false);
            avatar.transform.position = seat.AvatarAnchor.position + seat.transform.forward * 0.12f;
            avatar.transform.rotation = seat.transform.rotation;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(avatar.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            body.transform.localScale = new Vector3(0.42f, 0.62f, 0.42f);
            body.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("AvatarBody", new Color(0.18f, 0.34f, 0.48f));

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(avatar.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.72f, 0f);
            head.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            head.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("AvatarSkin", new Color(0.76f, 0.58f, 0.43f));

            AvatarFaceRig faceRig = CreateFaceRig(avatar, head.transform);

            GameObject leftHand = CreateHand("LeftHand", avatar.transform, new Vector3(-0.38f, 1.18f, 0.18f));
            GameObject rightHand = CreateHand("RightHand", avatar.transform, new Vector3(0.38f, 1.18f, 0.18f));

            GestureController gesture = avatar.AddComponent<GestureController>();
            gesture.AssignParts(head.transform, leftHand.transform, rightHand.transform);
            gesture.AssignFaceRig(faceRig);
            return gesture;
        }

        private AvatarFaceRig CreateFaceRig(GameObject avatar, Transform head)
        {
            Material eyeMaterial = RuntimeMaterialFactory.GetMaterial("FaceBlack", new Color(0.02f, 0.018f, 0.015f));
            Material browMaterial = RuntimeMaterialFactory.GetMaterial("BrowDark", new Color(0.08f, 0.045f, 0.025f));
            Material mouthMaterial = RuntimeMaterialFactory.GetMaterial("MouthRed", new Color(0.55f, 0.05f, 0.07f));
            Material cheekMaterial = RuntimeMaterialFactory.GetMaterial("CheekPink", new Color(0.95f, 0.38f, 0.34f));
            Material tongueMaterial = RuntimeMaterialFactory.GetMaterial("TonguePink", new Color(0.95f, 0.22f, 0.36f));
            Material noseMaterial = RuntimeMaterialFactory.GetMaterial("AvatarNose", new Color(0.68f, 0.48f, 0.36f));

            Transform leftEye = CreateFacePart("Left Eye", PrimitiveType.Sphere, head, new Vector3(-0.17f, 0.12f, 0.47f), new Vector3(0.095f, 0.048f, 0.022f), eyeMaterial, Quaternion.identity).transform;
            Transform rightEye = CreateFacePart("Right Eye", PrimitiveType.Sphere, head, new Vector3(0.17f, 0.12f, 0.47f), new Vector3(0.095f, 0.048f, 0.022f), eyeMaterial, Quaternion.identity).transform;
            Transform leftBrow = CreateFacePart("Left Brow", PrimitiveType.Cube, head, new Vector3(-0.17f, 0.24f, 0.46f), new Vector3(0.16f, 0.028f, 0.022f), browMaterial, Quaternion.Euler(0f, 0f, -8f)).transform;
            Transform rightBrow = CreateFacePart("Right Brow", PrimitiveType.Cube, head, new Vector3(0.17f, 0.24f, 0.46f), new Vector3(0.16f, 0.028f, 0.022f), browMaterial, Quaternion.Euler(0f, 0f, 8f)).transform;
            Transform mouth = CreateFacePart("Mouth", PrimitiveType.Cube, head, new Vector3(0f, -0.16f, 0.49f), new Vector3(0.22f, 0.035f, 0.022f), mouthMaterial, Quaternion.identity).transform;
            Transform nose = CreateFacePart("Nose", PrimitiveType.Sphere, head, new Vector3(0f, -0.01f, 0.52f), new Vector3(0.08f, 0.10f, 0.07f), noseMaterial, Quaternion.identity).transform;
            Transform leftCheek = CreateFacePart("Left Cheek", PrimitiveType.Sphere, head, new Vector3(-0.24f, -0.07f, 0.47f), new Vector3(0.085f, 0.055f, 0.025f), cheekMaterial, Quaternion.identity).transform;
            Transform rightCheek = CreateFacePart("Right Cheek", PrimitiveType.Sphere, head, new Vector3(0.24f, -0.07f, 0.47f), new Vector3(0.085f, 0.055f, 0.025f), cheekMaterial, Quaternion.identity).transform;
            Transform tongue = CreateFacePart("Tongue", PrimitiveType.Cube, head, new Vector3(0f, -0.235f, 0.51f), new Vector3(0.09f, 0.04f, 0.025f), tongueMaterial, Quaternion.identity).transform;

            AvatarFaceRig faceRig = avatar.AddComponent<AvatarFaceRig>();
            faceRig.AssignParts(leftEye, rightEye, leftBrow, rightBrow, mouth, nose, leftCheek, rightCheek, tongue);
            return faceRig;
        }

        private GameObject CreateFacePart(string partName, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Material material, Quaternion localRotation)
        {
            GameObject part = GameObject.CreatePrimitive(primitiveType);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = localRotation;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            return part;
        }

        private GameObject CreateHand(string name, Transform parent, Vector3 localPosition)
        {
            GameObject hand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hand.name = name;
            hand.transform.SetParent(parent, false);
            hand.transform.localPosition = localPosition;
            hand.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            hand.GetComponent<Renderer>().sharedMaterial = RuntimeMaterialFactory.GetMaterial("AvatarSkin", new Color(0.76f, 0.58f, 0.43f));
            return hand;
        }

        private void CreateSeatLabel(Seat seat)
        {
            GameObject labelObject = new GameObject(SeatUtility.GetDisplayName(seat.SeatId) + " Label");
            labelObject.transform.SetParent(generatedRoot, false);
            labelObject.transform.position = seat.LabelAnchor.position;
            labelObject.transform.rotation = Quaternion.LookRotation(Vector3.zero - labelObject.transform.position, Vector3.up);

            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = SeatUtility.GetShortName(seat.SeatId);
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.16f;
            label.fontSize = 48;
            label.color = seat.TeamId == TeamId.LocalTeam ? new Color(0.3f, 0.65f, 0.95f) : new Color(0.95f, 0.42f, 0.32f);
        }

        private Vector3 GetPlayedCardPosition(SeatId seat)
        {
            float y = GameConstants.TableHeight + 0.1f;
            switch (seat)
            {
                case SeatId.LocalPlayer:
                    return new Vector3(0f, y, -0.45f);
                case SeatId.RightRival:
                    return new Vector3(0.52f, y, 0f);
                case SeatId.Partner:
                    return new Vector3(0f, y, 0.45f);
                case SeatId.LeftRival:
                    return new Vector3(-0.52f, y, 0f);
                default:
                    return new Vector3(0f, y, 0f);
            }
        }

        private float GetPlayedCardYaw(SeatId seat)
        {
            switch (seat)
            {
                case SeatId.LocalPlayer:
                    return 0f;
                case SeatId.RightRival:
                    return -90f;
                case SeatId.Partner:
                    return 180f;
                case SeatId.LeftRival:
                    return 90f;
                default:
                    return 0f;
            }
        }

        private void ClearGeneratedObjects()
        {
            ClearAllCards();
            avatarGestures.Clear();

            if (generatedRoot != null)
            {
                DestroyObject(generatedRoot.gameObject);
                generatedRoot = null;
                cardsRoot = null;
            }
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
