using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using static Rootcraft.CollectNumber.Extensions;
using UnityEngine;
using System.Collections;

namespace Rootcraft.CollectNumber.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        public int Row = 5;
        public int Column = 5;

        [HideInInspector] public Piece[,] PieceGrid;
        [HideInInspector] public List<Piece> PopingList;

        [SerializeField] private GameObject _piecePrefab;
        [SerializeField] private Vector3 PieceMargin;

        private ResourceManager _rmInstance;

        protected override void Awake()
        {
            base.Awake();

            PieceGrid = new Piece[Row, Column];
            PopingList = new();
        }

        private void Start()
        {
            _rmInstance = ResourceManager.Instance;

            // Wait for level data
            _rmInstance.NumbersAndColorsLoadHandle.Completed += (_) => CreateGrid(Row, Column);
        }

        #region Grid
        private void CreateGrid(int maxRow, int maxColumn)
        {
            // Need to store column data since we are going to add to the same column in the next outer loop step.
            ChainedPiece[] chainedPiecePerColumn = new ChainedPiece[maxRow];

            for (int y = 0; y < maxRow; y++)
            {
                ChainedPiece currentRowChainedPiece = new();

                for (int x = 0; x < maxColumn; x++)
                {
                    ChainedPiece currentColumnChainedPiece = chainedPiecePerColumn[x];

                    Piece newPiece = PlaceNewPieceInGrid(ref currentColumnChainedPiece, ref currentRowChainedPiece, x, y);
                    UpdateChainedPiece(newPiece, ref currentRowChainedPiece);
                    UpdateChainedPiece(newPiece, ref currentColumnChainedPiece);
                    // Struct is passed by value, so we need to re-assign
                    chainedPiecePerColumn[x] = currentColumnChainedPiece;
                }
            }
        }

        public void FillGaps(PiecePos[] piecePoses)
        {
            int posesLengt = piecePoses.Length;
            Piece[] newPieces = new Piece[posesLengt];

            for (int i = 0; i < posesLengt; i++)
            {
                PiecePos pos = piecePoses[i];
                newPieces[i] = InstantiatePiece(pos.x, pos.y);
            }

            for (int i = 0; i < posesLengt; i++)
            {
                Piece newPiece = newPieces[i];
                if(newPiece == null)
                    break;
                
                newPiece.FindChainToPop();
            }
        }
        #endregion

        #region Piece
        private Piece InstantiatePiece(int row, int column, List<string> ignoreList = null)
        {
            NumbersAndColorsSO so = _rmInstance.GetRandomNumberAndColor(ignoreList);
            Piece newPiece = Instantiate(_piecePrefab).GetComponent<Piece>().Init(so, row, column);

            Vector3 pos = new(row * PieceMargin.x, column * PieceMargin.y, 0);
            newPiece.transform.position = pos;

            PieceGrid[row, column] = newPiece;

            return newPiece;
        }

        private Piece PlaceNewPieceInGrid(ref ChainedPiece currentColumnChainedPiece, ref ChainedPiece currentRowChainedPiece, int x, int y)
        {
            Piece newPiece;
            if (currentRowChainedPiece.ChainedCount < 1 && currentColumnChainedPiece.ChainedCount < 1)
                newPiece = InstantiatePiece(x, y);
            else
            {
                List<string> ignoreList = new();
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

            Debug.Log($"no: {chainedPiece.PieceNo}, count: {chainedPiece.ChainedCount}");
        }
        #endregion
    }
}