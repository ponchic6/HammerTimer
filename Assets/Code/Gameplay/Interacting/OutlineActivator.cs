using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Interacting
{
    public class OutlineActivator : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _overlapResults = new Collider[4];
        private SocketOutline _lastSocketOutline;
        private int _lastSocketId1;

        private void Update()
        {
            Vector3 interactionPosition = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(interactionPosition, _interactionRadius, _overlapResults, _socketLayer);
            
            if (socketCount == 0)
            {
                if (_lastSocketOutline != null)
                {
                    _lastSocketOutline.DisableOutline();
                    _lastSocketOutline = null;
                }
                return;
            }
            
            if (_overlapResults[0].TryGetComponent(out SocketOutline socketOutline))
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