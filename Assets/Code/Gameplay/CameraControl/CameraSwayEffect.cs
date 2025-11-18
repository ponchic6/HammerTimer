using UnityEngine;

namespace Code.Gameplay.CameraControl
{
    public class CameraSwayEffect : MonoBehaviour
    {
        [SerializeField] private float swaySpeed;
        [SerializeField] private float positionSwayAmplitude;

        private Vector3 _initialPosition;
        private float _timeOffset;

        private void Start()
        {
            _initialPosition = transform.localPosition;
            _timeOffset = Random.Range(0f, 1000f);
        }

        private void LateUpdate()
        {
            ApplySway();
        }

        private void ApplySway()
        {
            float time = Time.time * swaySpeed + _timeOffset;
            
            float posX = (Mathf.PerlinNoise(time + 10f, 0f) - 0.5f) * 2f * positionSwayAmplitude;
            float posY = (Mathf.PerlinNoise(0f, time + 20f) - 0.5f) * 2f * positionSwayAmplitude;
            float posZ = (Mathf.PerlinNoise(time + 30f, time + 40f) - 0.5f) * 2f * positionSwayAmplitude;

            Vector3 swayPosition = new Vector3(posX, posY, posZ);
            transform.localPosition = _initialPosition + swayPosition;
        }
    }
}
