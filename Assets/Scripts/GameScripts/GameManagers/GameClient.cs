using Unity.Mathematics;
using UnityEngine;

namespace Rootcraft.CollectNumber
{
    public class GameClient : Singleton<GameClient>
    {
        public Camera GameCamera;

        public void SetCameraPosZoomFromGrid(int row, int column)
        {
            //if(row <= 5 && column <= 5)
                //return;

            int big = math.max(row, column);
            Vector3 pos = GameCamera.transform.position; 
            pos.z += (big - 5) * -35;

            //if(row > 5)
                pos.x += GridManager.Instance.PieceMargin.x * (row - 5) * 0.5f;
            //if(column> 5)
                pos.y += GridManager.Instance.PieceMargin.y * (column - 5) * 0.5f;

            GameCamera.transform.position = pos;
        }
    }
}