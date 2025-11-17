using Terresquall;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class JoyStickInputService : IInputService
    {
        public Vector2 GetMoveDirection() =>
            VirtualJoystick.GetAxis();

        public bool GetInteractKeyDown() =>
            Input.GetKeyDown(KeyCode.Space);

        public bool GetInteractKey() =>
            Input.GetKey(KeyCode.Space);

        public void HoldKey(KeyCode keyCode)
        {
        }

        public void ReleaseKey(KeyCode keyCode)
        {
        }
    }
}