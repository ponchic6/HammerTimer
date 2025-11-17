using UnityEngine;

namespace Code.Infrastructure.Services
{
    public interface IReadOnlyInputService
    {
        public Vector2 GetMoveDirection();
        public bool GetInteractKeyDown();
        public bool GetInteractKey();
    }
}