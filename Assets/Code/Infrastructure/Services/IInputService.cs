using UnityEngine;

namespace Code.Infrastructure.Services
{
    public interface IInputService
    {
        public Vector2 GetMoveDirection();
        public bool GetInteractKeyDown();
        public bool GetInteractKey();
    }
}