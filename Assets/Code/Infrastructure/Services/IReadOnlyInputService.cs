using UnityEngine;

namespace Code.Infrastructure.Services
{
    public interface IReadOnlyInputService
    {
        public bool GetKey(KeyCode keyCode);
    }
}