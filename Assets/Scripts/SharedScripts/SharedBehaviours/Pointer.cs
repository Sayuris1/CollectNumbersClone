using UnityEngine;
using UnityEngine.InputSystem;

namespace Rootcraft.CollectNumber.InputSystem
{
    [RequireComponent(typeof(Collider))]
    public class Pointer : MonoBehaviour
    {
        public void Hold(InputAction.CallbackContext context)
        {
            Camera cam = GameClient.Instance.GameCamera;
            Vector2 pos = context.ReadValue<Vector2>();
            transform.position = cam.ScreenToWorldPoint(new Vector3 (pos.x, pos.y, cam.nearClipPlane));
        }

        public void Release(InputAction.CallbackContext context)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        public void Press(InputAction.CallbackContext context)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
