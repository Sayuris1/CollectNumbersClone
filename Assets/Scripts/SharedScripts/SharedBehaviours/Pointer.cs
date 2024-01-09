using Rootcraft.CollectNumber.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rootcraft.CollectNumber.InputSystem
{
    [RequireComponent(typeof(Collider))]
    public class Pointer : MonoBehaviour
    {
        private Collider _lastCollider;

        public void Hold(InputAction.CallbackContext context)
        {
            Camera cam = GameClient.Instance.GameCamera;
            Vector2 pointerPos = context.ReadValue<Vector2>();

            Vector3 newPos = cam.ScreenToWorldPoint(new Vector3 (pointerPos.x, pointerPos.y, -cam.transform.position.z));
            //newPos.z = 0;
            transform.position = newPos;
        }

        public void Release(InputAction.CallbackContext context)
        { 
            if(_lastCollider == null)
                return;
            
            if (_lastCollider.TryGetComponent(out Piece piece))
            {
                piece.Increase();
                piece.ShringUp();
            }
        }

        public void Press(InputAction.CallbackContext context)
        {}

        private void OnTriggerEnter(Collider other)
        {
            _lastCollider = other;

            if (other.TryGetComponent(out Piece piece))
                piece.ShringDown();
        }

        private void OnTriggerExit(Collider other)
        {
            if(_lastCollider == other)
                _lastCollider = null;

            if (other.TryGetComponent(out Piece piece))
                piece.ShringUp();
        }
    }
}
