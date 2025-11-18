using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Input.View
{
    public class ScreenInteractButton : MonoBehaviour
    {
        [SerializeField] private Canvas parentCanvas;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Button button;
        [SerializeField] private CanvasGroup buttonCanvasGroup;
        [SerializeField] private Rect touchAreaRect = new Rect(0, 0, 200, 200);
        [SerializeField] private bool hideWhenNotPressed = true;
        private bool _isPressed;
        private bool _isOneTimePressed;
        
        public bool IsPressed => _isPressed;
        public bool IsOneTimePressed => _isOneTimePressed;

        private void Awake()
        {
            if (!hideWhenNotPressed)
                return;
            
            buttonCanvasGroup.alpha = 0;
            buttonCanvasGroup.blocksRaycasts = false;
        }

        private void Update()
        {
            if (_isOneTimePressed)
                _isOneTimePressed = false;
            
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Began)
                {
                    if (IsTouchInArea(touch.position))
                    {
                        _isPressed = true;
                        _isOneTimePressed = true;
                        ShowButton(touch.position);
                        button.onClick?.Invoke();
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (_isPressed)
                    {
                        _isPressed = false;
                        HideButton();
                    }
                }
            }
            else if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = UnityEngine.Input.mousePosition;
                if (IsTouchInArea(mousePos))
                {
                    _isOneTimePressed = true;
                    _isPressed = true;
                    ShowButton(mousePos);
                    button.onClick?.Invoke();
                }
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (_isPressed)
                {
                    _isPressed = false;
                    HideButton();
                }
            }
        }

        private bool IsTouchInArea(Vector2 screenPosition)
        {
            return touchAreaRect.Contains(screenPosition);
        }

        private void ShowButton(Vector2 screenPosition)
        {
            if (hideWhenNotPressed)
            {
                buttonCanvasGroup.alpha = 1;
                buttonCanvasGroup.blocksRaycasts = true;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                screenPosition,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out Vector2 localPoint
            );

            rectTransform.localPosition = localPoint;
        }

        private void HideButton()
        {
            if (hideWhenNotPressed)
            {
                buttonCanvasGroup.alpha = 0;
                buttonCanvasGroup.blocksRaycasts = false;
            }
        }
        

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (parentCanvas == null)
                parentCanvas = GetComponentInParent<Canvas>();

            if (parentCanvas == null)
                return;

            Vector3[] corners = new Vector3[4];

            Vector3 bottomLeft = new Vector3(touchAreaRect.x, touchAreaRect.y, 0);
            Vector3 topRight = new Vector3(touchAreaRect.x + touchAreaRect.width, touchAreaRect.y + touchAreaRect.height, 0);

            corners[0] = bottomLeft; // Нижний левый
            corners[1] = new Vector3(topRight.x, bottomLeft.y, 0); // Нижний правый
            corners[2] = topRight; // Верхний правый
            corners[3] = new Vector3(bottomLeft.x, topRight.y, 0); // Верхний левый

            Gizmos.color = Color.red;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }

            Color fillColor = Color.red;
            fillColor.a = 0.1f;
            Gizmos.color = fillColor;

            Vector3 center = (bottomLeft + topRight) / 2;
            Vector3 size = new Vector3(touchAreaRect.width, touchAreaRect.height, 0.1f);
            Gizmos.DrawCube(center, size);

            UnityEditor.Handles.Label(center, "Touch Area");
        }
#endif
    }
}