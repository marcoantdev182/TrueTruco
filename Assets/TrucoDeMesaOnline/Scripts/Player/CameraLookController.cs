using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrucoDeMesaOnline
{
    public sealed class CameraLookController : MonoBehaviour
    {
        [Header("Limits")]
        [SerializeField] private float minYaw = -78f;
        [SerializeField] private float maxYaw = 78f;
        [SerializeField] private float minPitch = -42f;
        [SerializeField] private float maxPitch = 34f;

        [Header("Input")]
        [SerializeField] private float mouseSensitivity = 2.2f;
        [SerializeField] private float keyboardYawSpeed = 75f;
        [SerializeField] private float keyboardPitchSpeed = 55f;
        [SerializeField] private bool lockCursorOnClick;
        [SerializeField] private bool mouseLookRequiresRightButton = true;

        private float yaw;
        private float pitch;
        private bool legacyInputAvailable = true;

        public float NormalizedYaw
        {
            get { return Mathf.InverseLerp(minYaw, maxYaw, yaw); }
        }

        public void AttachToSeat(Seat seat)
        {
            transform.SetParent(seat.CameraAnchor, false);
            transform.localPosition = Vector3.zero;
            yaw = 0f;
            pitch = 2f;
            ApplyRotation();
        }

        private void Update()
        {
            UpdateCursorLock();
            ReadLookInput();
            ApplyRotation();
        }

        private void UpdateCursorLock()
        {
            if (!lockCursorOnClick)
            {
                return;
            }

            try
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            catch (InvalidOperationException)
            {
                legacyInputAvailable = false;
            }
        }

        private void ReadLookInput()
        {
            float yawDelta = 0f;
            float pitchDelta = 0f;

            if (legacyInputAvailable)
            {
                try
                {
                    bool canUseMouseLook = !mouseLookRequiresRightButton || Input.GetMouseButton(1);
                    if (canUseMouseLook && !IsPointerOverUi())
                    {
                        yawDelta += Input.GetAxis("Mouse X") * mouseSensitivity;
                        pitchDelta -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                    }
                }
                catch (InvalidOperationException)
                {
                    legacyInputAvailable = false;
                }
            }

            if (legacyInputAvailable)
            {
                try
                {
                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    {
                        yawDelta -= keyboardYawSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    {
                        yawDelta += keyboardYawSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                    {
                        pitchDelta -= keyboardPitchSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                    {
                        pitchDelta += keyboardPitchSpeed * Time.deltaTime;
                    }
                }
                catch (InvalidOperationException)
                {
                    legacyInputAvailable = false;
                }
            }

            yaw = Mathf.Clamp(yaw + yawDelta, minYaw, maxYaw);
            pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
