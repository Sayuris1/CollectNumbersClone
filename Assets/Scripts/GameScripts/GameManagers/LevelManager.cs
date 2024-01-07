using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using UnityEngine;

namespace Rootcraft.CollectNumber.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        public int Row = 5;
        public int Column = 5;

        public Piece[,] PieceGrid;

        [SerializeField] private GameObject _piecePrefab;

        private ResourceManager _rmInstance;

        protected override void Awake()
        {
            base.Awake();

            PieceGrid = new Piece[Row, Column];
        }

        private void Start()
        {
            _rmInstance = ResourceManager.Instance;

            // Wait for level data
            _rmInstance.NumbersAndColorsLoadHandle.Completed += (_) => CreateNewLevel(Row, Column);
        }

        private void Update()
        {
            
        }

        private void CreateNewLevel(int maxRow, int maxColumn)
        {
            // Need to store column data since we are going to add to the same column in the next outer loop step.
            ChainedPiece[] chainedPiecePerColumn = new ChainedPiece[maxRow];

            for (int y = 0; y < maxRow; y++)
            {
                ChainedPiece currentRowChainedPiece = new();

                for (int x = 0; x < maxColumn; x++)
                {
                    ChainedPiece currentColumnChainedPiece = chainedPiecePerColumn[x];
                    PlaceNewPieceInGrid(ref currentColumnChainedPiece, ref currentRowChainedPiece, x, y);
                }
            }
        }

        private void PlaceNewPieceInGrid(ref ChainedPiece currentColumnChainedPiece, ref ChainedPiece currentRowChainedPiece, int x, int y)
        {
            if (currentRowChainedPiece.ChainedCount < 2 && currentColumnChainedPiece.ChainedCount < 2)
            {
                Piece newPiece = InstantiatePiece(x, y);

                UpdateChainedPiece(newPiece, ref currentRowChainedPiece);
                UpdateChainedPiece(newPiece, ref currentColumnChainedPiece);
            }
            else
            {
                List<string> ignoreList = new() {currentRowChainedPiece.PieceNo.ToString()};
                Piece newPiece = InstantiatePiece(x, y, ignoreList);
                newPiece.PopNumbers.Add(currentRowChainedPiece.PieceNo);

                UpdateChainedPiece(newPiece, ref currentRowChainedPiece);
                UpdateChainedPiece(newPiece, ref currentColumnChainedPiece);
            }
        }

        private Piece InstantiatePiece(int row, int column, List<string> ignoreList = null)
        {
            NumbersAndColorsSO so = _rmInstance.GetRandomNumberAndColor(ignoreList);
            return Instantiate(_piecePrefab).GetComponent<Piece>().Init(so, row, column);
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
    }
}