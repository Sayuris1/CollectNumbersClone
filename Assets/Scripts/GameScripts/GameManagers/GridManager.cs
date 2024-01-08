using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using UnityEngine;
using Rootcraft.CollectNumber.Level;
using System;

namespace Rootcraft.CollectNumber
{
    public class GridManager : Singleton<GridManager>
    {
        [HideInInspector] public int Row = 5;
        [HideInInspector] public int Column = 5;
        public Vector3 PieceMargin;

        [HideInInspector] public Piece[,] PieceGrid;
        [HideInInspector] public List<Piece> PopingList;

        [SerializeField] private GameObject _piecePrefab;

        private ResourceManager _rmInstance;

        protected override void Awake()
        {
            base.Awake();

            PopingList = new();
        }

        private void Start()
        {
            _rmInstance = ResourceManager.Instance;
        }

        public void SetNextLevel(int row, int column)
        {
            Row = row;
            Column = column;

            PieceGrid = new Piece[Row, Column];
        }

        #region Grid
        public void CreateGrid(LevelsSO level)
        {
            // Need to store column data since we are going to add to the same column in the next outer loop step.
            ChainedPiece[] chainedPiecePerColumn = new ChainedPiece[Row];

            for (int y = 0; y < Column; y++)
            {
                ChainedPiece currentRowChainedPiece = new();

                for (int x = 0; x < Row; x++)
                {
                    ChainedPiece currentColumnChainedPiece = chainedPiecePerColumn[x];

                    List<string> levelIgnoreList = LevelManager.Instance.GetLevelIgnoreList(x, y);

                    Piece newPiece = PlaceNewPieceInGrid(ref currentColumnChainedPiece, ref currentRowChainedPiece, x, y, levelIgnoreList);
                    UpdateChainedPiece(newPiece, ref currentRowChainedPiece);
                    UpdateChainedPiece(newPiece, ref currentColumnChainedPiece);
                    // Struct is passed by value, so we need to re-assign
                    chainedPiecePerColumn[x] = currentColumnChainedPiece;
                }
            }

            AddSpesificPieceToPos(level);
        }

        public void FallDown(List<PiecePos> piecePoses)
        {
            EmptyByX[] rowEmptyArray = new EmptyByX[Row].Init();
            foreach (PiecePos pos in piecePoses)
                rowEmptyArray[pos.x] = FindFallCounts(rowEmptyArray[pos.x], pos);

            PiecePos[] toFillPosArray = new PiecePos[piecePoses.Count];
            List<Piece> falledPieceList = new();
            int fillIndex = 0;
            for (int i = 0; i < rowEmptyArray.Length; i++)
            {
                EmptyByX emptyBy = rowEmptyArray[i];

                if(emptyBy.EmptyCount == 0)
                    continue;

                for (int index = emptyBy.StartY + emptyBy.EmptyCount - 1; index >= 0; index--)
                {
                    if(index - emptyBy.EmptyCount < 0)
                    {
                        toFillPosArray[fillIndex].x = i;
                        toFillPosArray[fillIndex++].y = index;
                        continue;
                    }

                    PieceGrid[i, index] = PieceGrid[i, index - emptyBy.EmptyCount];
                    PieceGrid[i, index].SetRowColumn(i, index, PieceMargin);

                    falledPieceList.Add(PieceGrid[i, index]);
                }
            }

            FillGaps(toFillPosArray);

            foreach (Piece piece in falledPieceList)
                StartCoroutine(piece.FindChainToPop());
        }

        public void FillGaps(PiecePos[] piecePoses)
        {
            int posesLengt = piecePoses.Length;
            Piece[] newPieces = new Piece[posesLengt];

            for (int i = 0; i < posesLengt; i++)
            {
                PiecePos pos = piecePoses[i];
                newPieces[i] = InstantiatePiece(pos.x, pos.y, new());
            }

            for (int i = 0; i < posesLengt; i++)
            {
                Piece newPiece = newPieces[i];
                if(newPiece == null)
                    break;
                
                StartCoroutine(newPiece.FindChainToPop());
            }
        }

        private EmptyByX FindFallCounts(EmptyByX emptyBy, PiecePos pos)
        {
            if(emptyBy.StartY > pos.y)
                emptyBy.StartY = pos.y;

            emptyBy.EmptyCount++;

            return emptyBy;
        }
        #endregion

        #region Piece
        private Piece InstantiatePiece(int row, int column, List<string> ignoreList)
        {
            NumbersAndColorsSO so = _rmInstance.GetRandomNumberAndColor(ignoreList);
            Piece newPiece = Instantiate(_piecePrefab).GetComponent<Piece>().Init(so, row, column, PieceMargin);

            PieceGrid[row, column] = newPiece;

            return newPiece;
        }

        private Piece PlaceNewPieceInGrid(ref ChainedPiece currentColumnChainedPiece, ref ChainedPiece currentRowChainedPiece, int x, int y, List<string> ignoreList)
        {
            ignoreList ??= new();
            

            Piece newPiece;
            if (currentRowChainedPiece.ChainedCount < 1 && currentColumnChainedPiece.ChainedCount < 1)
                newPiece = InstantiatePiece(x, y, ignoreList);
            else
            {
                ignoreList.AddChainedPieceIfPop(currentRowChainedPiece).AddChainedPieceIfPop(currentColumnChainedPiece);

                newPiece = InstantiatePiece(x, y, ignoreList);
            }

            return newPiece;
        }

        private void UpdateChainedPiece(Piece piece, ref ChainedPiece chainedPiece)
        {
            if(chainedPiece.PieceNo == piece.PieceNo)
                chainedPiece.ChainedCount++;
            else
            {
                chainedPiece.PieceNo = piece.PieceNo;
                chainedPiece.ChainedCount = 0;
            }
        }

        public void AddSpesificPieceToPos(LevelsSO so)
        {
            foreach (PlacedNumber placed in so.PlacedNumberList)
            {
                Destroy(PieceGrid[placed.PlacedNumberX, placed.PlacedNumberY].gameObject);

                Piece newPiece = Instantiate(_piecePrefab).GetComponent<Piece>().Init(placed.PlacedNumberAndColor, placed.PlacedNumberX, placed.PlacedNumberY, PieceMargin);
                PieceGrid[placed.PlacedNumberX, placed.PlacedNumberY] = newPiece;
            }
        }
        #endregion
    }
}