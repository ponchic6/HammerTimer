using Terresquall;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class JoyStickInputService : IInputService
    {
        public Vector2 GetMoveDirection() =>
            VirtualJoystick.GetAxis();

        public bool GetInteractKeyDown()
        {
            bool interaction = VirtualJoystick.GetInteraction();
            if (interaction)
            {
                VirtualJoystick.ResetInteraction();
                return true;
            }

            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool GetInteractKey()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public void HoldKey(KeyCode keyCode)
        {
        }

        public void ReleaseKey(KeyCode keyCode)
        {
        }
    }
}