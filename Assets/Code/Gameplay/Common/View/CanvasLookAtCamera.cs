using UnityEngine;

namespace Code.Gameplay.Common.View
{
    public class CanvasLookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool maintainConstantSize = true;
        [SerializeField] private float fixedSize;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (maintainConstantSize)
            {
                Vector3 toCanvas = transform.position - _mainCamera.transform.position;
                float perpendicularDistance = Vector3.Dot(toCanvas, _mainCamera.transform.forward);

                if (perpendicularDistance > 0)
                {
                    float size = perpendicularDistance * fixedSize * Mathf.Tan(_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    transform.localScale = Vector3.one * size;
                }
            }

            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }
    }
}

