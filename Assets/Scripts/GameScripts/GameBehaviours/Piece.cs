using System;
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
        public Piece Init(NumbersAndColorsSO so, int row, int column, Vector3 margin)
        {
            SetRowColumn(row, column, margin);

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

            _lmInstance.StartCoroutine(FindChainToPop());
        }

        private void ApplyData(NumbersAndColorsSO so)
        {
            PieceNo = so.Number;

            _tmp.text = PieceNo.ToString();
            _renderer.color = so.Color;
        }

        public void SetRowColumn(int row, int column, Vector3 margin)
        {
            Debug.Log($"old: {ColumnInGrid} new: {column}");
            RowInGrid = row;
            ColumnInGrid = column;

            Vector3 pos = new(row * margin.x, column * margin.y, 0);
            transform.position = pos;
        }
        #endregion

        #region Pop
        private void Pop()
        {
            if(gameObject == null)
                return;

            _lmInstance.PopingList.RemoveAll(piece => piece == this);
            Destroy(gameObject);
        }

        private IEnumerator PopThenFill(int chainCount, List<Piece> popList, Action<List<PiecePos>> getPosInGrid)
        {
            // Copy grid poses before delete
            List<PiecePos> posInGrid = new();

            if(chainCount <= 3)
            {
                getPosInGrid(posInGrid);
                yield break;
            }

            // Store poping pieces so we can ignore them
            _lmInstance.PopingList.AddRange(popList);

            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < popList.Count; i++)
            {
                Piece piece = popList[i];

                posInGrid.Add(new(){x = piece.RowInGrid, y = piece.ColumnInGrid});

                piece._renderer.color = Color.white;
            }

            for (int i = 0; i < popList.Count; i++)
            {
                yield return new WaitForSeconds(0.1f);
                Piece piece = popList[i];
                piece.Pop();
            }

            getPosInGrid(posInGrid);
        }

        public IEnumerator FindChainToPop()
        {
            int completedAsyncCount = 0;
            List<Piece> popList = new(){this};
            List<PiecePos> fallDownPosList = new();

            int rowChainCount = ChainPop(1, 0, ref popList) + ChainPop(-1, 0, ref popList);
            _lmInstance.StartCoroutine(PopThenFill(rowChainCount, popList, (list) =>
            {
                fallDownPosList.AddRange(list);
                completedAsyncCount++;
            }));

            // Reset
            popList = new(){this};

            int columnChainCount = ChainPop(0, 1, ref popList) + ChainPop(0, -1, ref popList);

            // Need to delete self to prevent it from double poping
            if(rowChainCount > 3)
                popList.RemoveAt(0);

            _lmInstance.StartCoroutine(PopThenFill(columnChainCount, popList, (list) =>
            {
                fallDownPosList.AddRange(list);
                completedAsyncCount++;
            }));

            yield return new WaitUntil(() => completedAsyncCount == 2);

            Debug.Log($"size out {fallDownPosList.Count}");
            _lmInstance.FallDown(fallDownPosList);
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
