using UnityEngine;

namespace Code.Infrastructure.Services
{
    public interface IInputService : IReadOnlyInputService
    {
        public void HoldKey(KeyCode keyCode);
        public void ReleaseKey(KeyCode keyCode);
    }
}