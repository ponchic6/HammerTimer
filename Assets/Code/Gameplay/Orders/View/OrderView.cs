using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Orders.View
{
    public class OrderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timer;
        [SerializeField] private Image icon;
        private int _entityId;

        public void SetupOrder(int entityId)
        {
            _entityId = entityId;
        }

        public void SetImage(Sprite sprite)
        {
            icon.sprite = sprite;
        }
        
        public void UpdateTimer(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timer.text = $"<mspace=0.35em>{minutes}:{seconds:D2}</mspace>";
        }
    }
}