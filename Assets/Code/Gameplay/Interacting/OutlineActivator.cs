using Code.Gameplay.Produce.View;
using UnityEngine;

namespace Code.Gameplay.Interacting
{
    public class OutlineActivator : MonoBehaviour
    {
        [SerializeField] private float _interactionDistance;
        [SerializeField] private LayerMask _socketLayer;
        private SocketOutline _lastSocketOutline;

        private void Update()
        {
            RaycastHit hit;
            bool rayHit = Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, _interactionDistance, _socketLayer);

            if (!rayHit)
            {
                if (_lastSocketOutline != null)
                {
                    _lastSocketOutline.DisableOutline();
                    _lastSocketOutline = null;
                }
                return;
            }

            if (hit.collider.TryGetComponent(out SocketOutline socketOutline))
            {
                if (_lastSocketOutline == socketOutline)
                    return;

                socketOutline.EnableOutline();

                if (_lastSocketOutline != null)
                    _lastSocketOutline.DisableOutline();

                _lastSocketOutline = socketOutline;
            }
        }
    }
}