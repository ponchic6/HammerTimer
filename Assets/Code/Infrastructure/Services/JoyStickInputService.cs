using Code.Gameplay.Input.View;
using Terresquall;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class JoyStickInputService : IInputService
    {
        private ScreenInteractButton _screenInteractButton;

        public Vector2 GetMoveDirection() =>
            VirtualJoystick.GetAxis();

        public bool GetInteractKeyDown()
        {
            if (_screenInteractButton == null) 
                _screenInteractButton = Object.FindFirstObjectByType<ScreenInteractButton>();

            return _screenInteractButton.IsOneTimePressed;
        }

        public bool GetInteractKey()
        {
            if (_screenInteractButton == null) 
                _screenInteractButton = Object.FindFirstObjectByType<ScreenInteractButton>();

            return _screenInteractButton.IsPressed;
        }
    }
}