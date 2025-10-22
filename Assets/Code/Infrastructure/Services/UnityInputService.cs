using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class UnityInputService : IInputService
    {
        private readonly HashSet<KeyCode> _simulatedHeldKeys = new();

        public bool GetKey(KeyCode keyCode)
        {
            if (_simulatedHeldKeys.Contains(keyCode))
                return true;

            return Input.GetKey(keyCode);
        }

        public bool GetInteractKeyDown() =>
            Input.GetKeyDown(KeyCode.Space);

        public bool GetInteractKey() =>
            Input.GetKey(KeyCode.Space);

        public void HoldKey(KeyCode keyCode) =>
            _simulatedHeldKeys.Add(keyCode);

        public void ReleaseKey(KeyCode keyCode) =>
            _simulatedHeldKeys.Remove(keyCode);
    }
}