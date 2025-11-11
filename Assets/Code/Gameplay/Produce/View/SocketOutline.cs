using UnityEngine;

namespace Code.Gameplay.Produce.View
{
    public class SocketOutline : MonoBehaviour
    {
        [SerializeField] private GameObject outline;

        public void EnableOutline()
        {
            outline.SetActive(true);
        }
        
        public void DisableOutline()
        {
            outline.SetActive(false);
        }
    }
}