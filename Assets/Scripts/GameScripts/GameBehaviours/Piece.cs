using System.Collections;
using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using TMPro;
using UnityEngine;

namespace Rootcraft.CollectNumber.Level
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Piece : MonoBehaviour
    {
        [HideInInspector] public int PieceNo;

        [HideInInspector] public int RowInGrid;
        [HideInInspector] public int ColumnInGrid;

        [SerializeField] private TMP_Text _tmp;
        private SpriteRenderer _renderer;
        private static LevelManager _lmInstance;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if(_lmInstance == null)
                _lmInstance = LevelManager.Instance;
        }

        #region Data
        public Piece Init(NumbersAndColorsSO so, int row, int column)
        {
            RowInGrid = row;
            ColumnInGrid = column;

            ApplyData(so);

            return this;
        }

        public void Increase()
        {
            if(_lmInstance.PopingList.Contains(this))
                return;

            int newPieceNo = PieceNo + 1;
            if(PieceNo < 0 || newPieceNo > ResourceManager.Instance.NumbersAndColorsLenght)   
            {
                PieceNo = -1;

                _tmp.text = "X";
                _renderer.color = Color.cyan;
                return;
            }

            NumbersAndColorsSO so = ResourceManager.Instance.GetNumberAndColor(newPieceNo.ToString());
            ApplyData(so);

            FindChainToPop();
        }

        private void ApplyData(NumbersAndColorsSO so)
        {
            PieceNo = so.Number;

            _tmp.text = PieceNo.ToString();
            _renderer.color = so.Color;
        }
        #endregion

        #region Pop
        private void Pop()
        {
            _lmInstance.PopingList.RemoveAll(piece => piece == this);
            Destroy(gameObject);
        }

        private IEnumerator PopThenFill(int chainCount, List<Piece> popList)
        {
            if(chainCount > 3)
            {
                // Copy grid poses before delete
                PiecePos[] posInGrid = new PiecePos[popList.Count];
                // Store poping pieces so we can ignore them
                _lmInstance.PopingList.AddRange(popList);

                yield return new WaitForSeconds(0.2f);

                for (int i = 0; i < popList.Count; i++)
                {
                    Piece piece = popList[i];

                    posInGrid[i].x = piece.RowInGrid;
                    posInGrid[i].y = piece.ColumnInGrid;

                    piece._renderer.color = Color.white;
                }

                for (int i = 0; i < popList.Count; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    Piece piece = popList[i];
                    piece.Pop();
                }

                yield return new WaitForSeconds(0.3f);
                _lmInstance.FillGaps(posInGrid);
            }
        }

        public void FindChainToPop()
        {
            List<Piece> popList = new(){this};

            int rowChainCount = ChainPop(1, 0, ref popList) + ChainPop(-1, 0, ref popList);
            _lmInstance.StartCoroutine(PopThenFill(rowChainCount, popList));

            // Reset
            popList = new(){this};

            int columnChainCount = ChainPop(0, 1, ref popList) + ChainPop(0, -1, ref popList);

            // Need to delete self to prevent it from double poping
            if(rowChainCount > 3)
                popList.RemoveAt(0);

            _lmInstance.StartCoroutine(PopThenFill(columnChainCount, popList));
        }

        // Return 0 if no chain
        // Return 1 end of chain
        // Return 1 + self if chain continues
        private int ChainPop(int deltaX, int deltaY, ref List<Piece> pieceList)
        {
            Piece[,] grid = _lmInstance.PieceGrid;
            int xPos = RowInGrid + deltaX;
            int yPos = ColumnInGrid + deltaY;

            if(pieceList[0].PieceNo != PieceNo)
                return 0;
            else if(pieceList[0] != this && _lmInstance.PopingList.Contains(this)) // Ignore if in ignore list
                return 0;
            else // This piece can pop
            {
                if(!pieceList.Contains(this))
                    pieceList.Add(this);

                if(grid.GetLength(0) - 1 < xPos || grid.GetLength(1) - 1 < yPos)
                    return 1;
                else if(0 > xPos || 0 > yPos)
                    return 1;
                else if(grid[xPos, yPos] == null) // Piece already poped
                    return 0;
            }

            return 1 + grid[xPos, yPos].ChainPop(deltaX, deltaY, ref pieceList);
        }
        #endregion

    }
}
