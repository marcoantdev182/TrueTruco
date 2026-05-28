using UnityEngine;

namespace TrucoDeMesaOnline
{
    public sealed class AvatarFaceRig : MonoBehaviour
    {
        [SerializeField] private Transform leftEye;
        [SerializeField] private Transform rightEye;
        [SerializeField] private Transform leftBrow;
        [SerializeField] private Transform rightBrow;
        [SerializeField] private Transform mouth;
        [SerializeField] private Transform nose;
        [SerializeField] private Transform leftCheek;
        [SerializeField] private Transform rightCheek;
        [SerializeField] private Transform tongue;

        private Renderer tongueRenderer;

        private Vector3 leftEyeScale;
        private Vector3 rightEyeScale;
        private Vector3 leftBrowPosition;
        private Vector3 rightBrowPosition;
        private Quaternion leftBrowRotation;
        private Quaternion rightBrowRotation;
        private Vector3 mouthScale;
        private Vector3 noseScale;
        private Vector3 leftCheekScale;
        private Vector3 rightCheekScale;
        private Vector3 tonguePosition;
        private Vector3 tongueScale;

        public void AssignParts(
            Transform leftEyeTransform,
            Transform rightEyeTransform,
            Transform leftBrowTransform,
            Transform rightBrowTransform,
            Transform mouthTransform,
            Transform noseTransform,
            Transform leftCheekTransform,
            Transform rightCheekTransform,
            Transform tongueTransform)
        {
            leftEye = leftEyeTransform;
            rightEye = rightEyeTransform;
            leftBrow = leftBrowTransform;
            rightBrow = rightBrowTransform;
            mouth = mouthTransform;
            nose = noseTransform;
            leftCheek = leftCheekTransform;
            rightCheek = rightCheekTransform;
            tongue = tongueTransform;
            tongueRenderer = tongue != null ? tongue.GetComponent<Renderer>() : null;
            CacheRestPose();
            ResetPose();
        }

        public void CacheRestPose()
        {
            if (leftEye != null)
            {
                leftEyeScale = leftEye.localScale;
            }

            if (rightEye != null)
            {
                rightEyeScale = rightEye.localScale;
            }

            if (leftBrow != null)
            {
                leftBrowPosition = leftBrow.localPosition;
                leftBrowRotation = leftBrow.localRotation;
            }

            if (rightBrow != null)
            {
                rightBrowPosition = rightBrow.localPosition;
                rightBrowRotation = rightBrow.localRotation;
            }

            if (mouth != null)
            {
                mouthScale = mouth.localScale;
            }

            if (nose != null)
            {
                noseScale = nose.localScale;
            }

            if (leftCheek != null)
            {
                leftCheekScale = leftCheek.localScale;
            }

            if (rightCheek != null)
            {
                rightCheekScale = rightCheek.localScale;
            }

            if (tongue != null)
            {
                tonguePosition = tongue.localPosition;
                tongueScale = tongue.localScale;
            }
        }

        public void ApplySignalPose(SignalType signal, float weight)
        {
            ResetPose();

            switch (signal)
            {
                case SignalType.WinkZap:
                    SetScale(leftEye, Vector3.Lerp(leftEyeScale, new Vector3(leftEyeScale.x, leftEyeScale.y * 0.12f, leftEyeScale.z), weight));
                    Move(leftBrow, leftBrowPosition + Vector3.down * 0.018f, weight);
                    Rotate(leftBrow, Quaternion.Euler(0f, 0f, 10f), weight);
                    break;
                case SignalType.RaiseEyebrowHearts:
                    Move(leftBrow, leftBrowPosition + Vector3.up * 0.045f, weight);
                    Move(rightBrow, rightBrowPosition + Vector3.up * 0.045f, weight);
                    Rotate(leftBrow, Quaternion.Euler(0f, 0f, -9f), weight);
                    Rotate(rightBrow, Quaternion.Euler(0f, 0f, 9f), weight);
                    break;
                case SignalType.PuffCheekSpades:
                    SetScale(leftCheek, Vector3.Lerp(leftCheekScale, leftCheekScale * 1.85f, weight));
                    SetScale(rightCheek, Vector3.Lerp(rightCheekScale, rightCheekScale * 1.85f, weight));
                    SetScale(mouth, Vector3.Lerp(mouthScale, new Vector3(mouthScale.x * 0.55f, mouthScale.y * 1.2f, mouthScale.z), weight));
                    break;
                case SignalType.TongueOrNoseDiamonds:
                    SetTongueVisible(true);
                    Move(tongue, tonguePosition + Vector3.forward * 0.055f + Vector3.down * 0.018f, weight);
                    SetScale(tongue, Vector3.Lerp(Vector3.zero, tongueScale, weight));
                    break;
                case SignalType.NoseTouchAce:
                    SetScale(nose, Vector3.Lerp(noseScale, noseScale * 1.55f, weight));
                    break;
                case SignalType.ChinHandKing:
                    SetScale(mouth, Vector3.Lerp(mouthScale, new Vector3(mouthScale.x * 0.72f, mouthScale.y * 0.8f, mouthScale.z), weight));
                    break;
                case SignalType.EarHandQueen:
                    Move(rightBrow, rightBrowPosition + Vector3.up * 0.025f, weight);
                    break;
                case SignalType.EarToChinJack:
                    SetScale(rightEye, Vector3.Lerp(rightEyeScale, rightEyeScale * 1.22f, weight));
                    break;
            }
        }

        public void ResetPose()
        {
            SetScale(leftEye, leftEyeScale);
            SetScale(rightEye, rightEyeScale);
            SetLocalPosition(leftBrow, leftBrowPosition);
            SetLocalPosition(rightBrow, rightBrowPosition);
            SetLocalRotation(leftBrow, leftBrowRotation);
            SetLocalRotation(rightBrow, rightBrowRotation);
            SetScale(mouth, mouthScale);
            SetScale(nose, noseScale);
            SetScale(leftCheek, leftCheekScale);
            SetScale(rightCheek, rightCheekScale);
            SetLocalPosition(tongue, tonguePosition);
            SetScale(tongue, tongueScale);
            SetTongueVisible(false);
        }

        private void Move(Transform target, Vector3 targetPosition, float weight)
        {
            if (target != null)
            {
                target.localPosition = Vector3.Lerp(target.localPosition, targetPosition, weight);
            }
        }

        private void Rotate(Transform target, Quaternion deltaRotation, float weight)
        {
            if (target != null)
            {
                target.localRotation = Quaternion.Slerp(target.localRotation, target.localRotation * deltaRotation, weight);
            }
        }

        private static void SetLocalPosition(Transform target, Vector3 value)
        {
            if (target != null)
            {
                target.localPosition = value;
            }
        }

        private static void SetLocalRotation(Transform target, Quaternion value)
        {
            if (target != null)
            {
                target.localRotation = value;
            }
        }

        private static void SetScale(Transform target, Vector3 value)
        {
            if (target != null)
            {
                target.localScale = value;
            }
        }

        private void SetTongueVisible(bool visible)
        {
            if (tongueRenderer != null)
            {
                tongueRenderer.enabled = visible;
            }
        }
    }
}

