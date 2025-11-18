using Code.Gameplay.Produce.View;
using UnityEngine;

namespace Code.Gameplay.Interacting
{
    public class OutlineActivator : MonoBehaviour
    {
        [SerializeField] private float _interactionDistance;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _results = new Collider[4];
        private SocketOutline _lastSocketOutline;

        private void Update()
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, _interactionDistance, _results, _socketLayer);

            if (size == 0)
            {
                if (_lastSocketOutline != null)
                {
                    _lastSocketOutline.DisableOutline();
                    _lastSocketOutline = null;
                }
                return;
            }

            SocketOutline closestSocketOutline = null;
            float closestDistance = float.MaxValue;

            foreach (Collider collider1 in _results)
            {
                if (collider1 == null)
                    continue;
                
                if (collider1.TryGetComponent(out SocketOutline socketOutline))
                {
                    float distance = Vector3.Distance(transform.position, collider1.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSocketOutline = socketOutline;
                    }
                }
            }

            if (closestSocketOutline == null)
            {
                if (_lastSocketOutline != null)
                {
                    _lastSocketOutline.DisableOutline();
                    _lastSocketOutline = null;
                }
                return;
            }

            if (_lastSocketOutline == closestSocketOutline)
                return;

            closestSocketOutline.EnableOutline();

            if (_lastSocketOutline != null)
                _lastSocketOutline.DisableOutline();

            _lastSocketOutline = closestSocketOutline;
        }
    }
}