using System.Collections;
using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class GestureController : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        private Vector3 headStartScale;
        private Vector3 headStartLocalPosition;
        private Vector3 leftHandStart;
        private Vector3 rightHandStart;
        private Coroutine gestureRoutine;

        public void AssignParts(Transform headTransform, Transform leftHandTransform, Transform rightHandTransform)
        {
            head = headTransform;
            leftHand = leftHandTransform;
            rightHand = rightHandTransform;
            CacheStarts();
        }

        public void PlayGesture(SignalType signal)
        {
            CacheStarts();

            if (gestureRoutine != null)
            {
                StopCoroutine(gestureRoutine);
            }

            gestureRoutine = StartCoroutine(PlayGestureRoutine(signal));
        }

        private IEnumerator PlayGestureRoutine(SignalType signal)
        {
            float duration = 0.75f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Sin((elapsed / duration) * Mathf.PI);
                ApplyGesturePose(signal, t);
                yield return null;
            }

            ResetPose();
            gestureRoutine = null;
        }

        private void ApplyGesturePose(SignalType signal, float weight)
        {
            if (head == null)
            {
                return;
            }

            switch (signal)
            {
                case SignalType.WinkZap:
                    head.localScale = Vector3.Lerp(headStartScale, new Vector3(headStartScale.x, headStartScale.y * 0.72f, headStartScale.z), weight);
                    break;
                case SignalType.RaiseEyebrowHearts:
                    head.localPosition = Vector3.Lerp(headStartLocalPosition, headStartLocalPosition + Vector3.up * 0.08f, weight);
                    break;
                case SignalType.PuffCheekSpades:
                    head.localScale = Vector3.Lerp(headStartScale, new Vector3(headStartScale.x * 1.18f, headStartScale.y, headStartScale.z * 1.08f), weight);
                    break;
                case SignalType.TongueOrNoseDiamonds:
                case SignalType.NoseTouchAce:
                    MoveRightHand(new Vector3(0f, 1.55f, 0.22f), weight);
                    break;
                case SignalType.OneShoulderThree:
                    MoveRightHand(rightHandStart + Vector3.up * 0.18f, weight);
                    break;
                case SignalType.BothShouldersDoubleThree:
                    MoveRightHand(rightHandStart + Vector3.up * 0.18f, weight);
                    MoveLeftHand(leftHandStart + Vector3.up * 0.18f, weight);
                    break;
                case SignalType.ChinHandKing:
                    MoveRightHand(new Vector3(0.03f, 1.38f, 0.24f), weight);
                    break;
                case SignalType.EarHandQueen:
                    MoveRightHand(new Vector3(0.28f, 1.62f, 0.05f), weight);
                    break;
                case SignalType.EarToChinJack:
                    MoveRightHand(Vector3.Lerp(new Vector3(0.28f, 1.62f, 0.05f), new Vector3(0.03f, 1.38f, 0.24f), weight), 1f);
                    break;
            }
        }

        private void MoveRightHand(Vector3 target, float weight)
        {
            if (rightHand != null)
            {
                rightHand.localPosition = Vector3.Lerp(rightHandStart, target, weight);
            }
        }

        private void MoveLeftHand(Vector3 target, float weight)
        {
            if (leftHand != null)
            {
                leftHand.localPosition = Vector3.Lerp(leftHandStart, target, weight);
            }
        }

        private void CacheStarts()
        {
            if (head != null)
            {
                headStartScale = head.localScale;
                headStartLocalPosition = head.localPosition;
            }

            if (leftHand != null)
            {
                leftHandStart = leftHand.localPosition;
            }

            if (rightHand != null)
            {
                rightHandStart = rightHand.localPosition;
            }
        }

        private void ResetPose()
        {
            if (head != null)
            {
                head.localScale = headStartScale;
                head.localPosition = headStartLocalPosition;
            }

            if (leftHand != null)
            {
                leftHand.localPosition = leftHandStart;
            }

            if (rightHand != null)
            {
                rightHand.localPosition = rightHandStart;
            }
        }
    }
}

