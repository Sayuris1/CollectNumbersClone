using Rootcraft.CollectNumber;
using UnityEngine;

namespace Rootcraft
{
    [RequireComponent(typeof(Camera))]
    public class CameraRegisterer : MonoBehaviour
    {
        public enum CameraTypes
        {
            Game,
            UI,
        }

        public CameraTypes CameraType;

        private void Start()
        {
            if(CameraType == CameraTypes.Game)
                GameClient.Instance.RegisterGameCamera(GetComponent<Camera>());
        }
    }
}