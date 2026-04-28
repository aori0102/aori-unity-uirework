using UnityEngine;

namespace Aori
{
    [DisallowMultipleComponent]
    public sealed class WorldCanvasModification : MonoBehaviour
    {
        [Header("- Facing")]
        [Header("+ Facing Option")]
        [SerializeField]
        private FacingIndex m_facingIndex;

        [Header("+ Rotation Lock")]
        [SerializeField]
        private bool m_freezeXRotation;

        [SerializeField]
        private bool m_freezeYRotation;

        [SerializeField]
        private bool m_freezeZRotation;

        [Header("- Fluctuation")]
        [Header("+ Vertical Fluctuation")]
        [SerializeField]
        private bool m_enableVerticalFluctuation;

        [SerializeField]
        private float m_verticalAmplitude;

        [SerializeField]
        private float m_verticalFrequency;

        [Header("+ Horizontal Fluctuation")]
        [SerializeField]
        private bool m_enableHorizontalFluctuation;

        [SerializeField]
        private float m_horizontalAmplitude;

        [SerializeField]
        private float m_horizontalFrequency;


        private Camera m_camera;
        private Vector3 m_originalPosition;
        private Vector3 m_originalEuler;
        private Vector3 m_verticalFluctuationOffset;
        private Vector3 m_horizontalFluctuationOffset;

        private void Awake()
        {
            m_camera = Camera.main;

            m_originalPosition = transform.localPosition;
            m_originalEuler = transform.localEulerAngles;
        }

        private void Update()
        {
            HandleLook();
            FixRotation();
            HandleVerticalFluctuation();
            HandleHorizontalFluctuation();
            FinalizeOffset();
        }

        private void FinalizeOffset()
        {
            transform.localPosition = m_originalPosition + m_verticalFluctuationOffset + m_horizontalFluctuationOffset;
        }

        private void FixRotation()
        {
            var euler = transform.localEulerAngles;
            if (m_freezeXRotation)
            {
                euler.x = m_originalEuler.x;
            }

            if (m_freezeYRotation)
            {
                euler.y = m_originalEuler.y;
            }

            if (m_freezeXRotation)
            {
                euler.z = m_originalEuler.z;
            }

            transform.localEulerAngles = euler;
        }

        private void HandleLook()
        {
            var delta = m_camera.transform.position - transform.position;
            switch (m_facingIndex)
            {
                case FacingIndex.FacingCamera:
                    transform.forward = delta;
                    break;

                case FacingIndex.FacingCameraInversed:
                    transform.forward = -delta;
                    break;

                case FacingIndex.None:
                default:
                    break;
            }
        }

        private void HandleVerticalFluctuation()
        {
            if (!m_enableVerticalFluctuation)
            {
                m_verticalFluctuationOffset = Vector3.zero;
                return;
            }

            var fluctuation = GetFluctuation(m_verticalAmplitude, m_verticalFrequency);
            m_verticalFluctuationOffset = Vector3.up * fluctuation;
        }

        private void HandleHorizontalFluctuation()
        {
            if (!m_enableHorizontalFluctuation)
            {
                m_horizontalFluctuationOffset = Vector3.zero;
                return;
            }

            var fluctuation = GetFluctuation(m_horizontalAmplitude, m_horizontalFrequency);
            m_horizontalFluctuationOffset = Vector3.right * fluctuation;
        }

        private float GetFluctuation(float amplitude, float frequency)
        {
            return Mathf.Cos(Time.time * frequency * 2f * Mathf.PI) * amplitude;
        }
    }
}