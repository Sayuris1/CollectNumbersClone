using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using TMPro;
using UnityEngine;

namespace Rootcraft.CollectNumber.Level
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Piece : MonoBehaviour
    {
        [HideInInspector] public List<int> PopNumbers;
        [HideInInspector] public int PieceNo;

        [HideInInspector] public int RowInGrid;
        [HideInInspector] public int ColumnInGrid;

        [SerializeField] private TMP_Text _tmp;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            PopNumbers = new();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public Piece Init(NumbersAndColorsSO so, int row, int column)
        {
            RowInGrid = row;
            ColumnInGrid = column;

            ApplyData(so);

            return this;
        }

        public void Increase()
        {
            int newPieceNo = PieceNo + 1;
            if(newPieceNo > ResourceManager.Instance.NumbersAndColorsLenght)   
            {
                _tmp.text = "X";
                _renderer.color = Color.cyan;
                return;
            }

            NumbersAndColorsSO so = ResourceManager.Instance.GetNumberAndColor(newPieceNo.ToString());
            ApplyData(so);

            FindChainToPop();
        }

        private int ChainPop(int deltaX, int deltaY, ref List<Piece> pieceList)
        {
            Piece[,] grid = LevelManager.Instance.PieceGrid;
            int xPos = RowInGrid + deltaX;
            int yPos = ColumnInGrid + deltaY;

            if(pieceList[0].PieceNo != PieceNo)
                return 0;
            else
            {
                pieceList.Add(this);

                if(grid.GetLength(0) - 1 < xPos || grid.GetLength(1) - 1 < yPos)
                    return 1;
                else if(0 > xPos || 0 > yPos)
                    return 1;
            }

            return 1 + grid[RowInGrid + deltaX, ColumnInGrid + deltaY].ChainPop(deltaX, deltaY, ref pieceList);
        }

        private void ApplyData(NumbersAndColorsSO so)
        {
            PieceNo = so.Number;

            _tmp.text = PieceNo.ToString();
            _renderer.color = so.Color;
        }

        private void Pop()
        {
            Destroy(gameObject);
        }


        private void FindChainToPop()
        {
            List<Piece> popList = new(){this};

            int rowChainCount = ChainPop(1, 0, ref popList) + ChainPop(-1, 0, ref popList);
            Debug.Log($"row: {rowChainCount }");
            if(rowChainCount > 3)
                foreach (Piece piece in popList)
                    piece.Pop();

            // Reset
            popList = new(){this};

            int columnChainCount = ChainPop(0, 1, ref popList) + ChainPop(0, -1, ref popList);
            Debug.Log($"column: {columnChainCount }");
            if(columnChainCount > 3)
                foreach (Piece piece in popList)
                    piece.Pop();
        }

    }
}
