using Unity.Mathematics;
using UnityEngine;

namespace Rootcraft.CollectNumber
{
    public class GameClient : Singleton<GameClient>
    {
        [HideInInspector] public Camera GameCamera {get; private set;}
        
        private Vector3 _gameCamerastartPos;

        public void RegisterGameCamera(Camera cam)
        {
            GameCamera = cam;
            _gameCamerastartPos = cam.transform.position;
        }

        public void SetCameraPosZoomFromGrid(int row, int column)
        {
            int big = math.max(row, column);
            Vector3 pos = _gameCamerastartPos; 
            pos.z += (big - 5) * -35;

            pos.x += GridManager.Instance.PieceMargin.x * (row - 5) * 0.5f;
            pos.y += GridManager.Instance.PieceMargin.y * (column - 5) * 0.5f;

            GameCamera.transform.position = pos;
        }
    }
}