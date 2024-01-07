using System.Collections.Generic;
using UnityEngine;

namespace Rootcraft.CollectNumber.Level
{
    public class Piece : MonoBehaviour
    {
        [HideInInspector] public List<int> PopNumbers;
        [HideInInspector] public int PieceNo;

        [HideInInspector] public int RowInGrid;
        [HideInInspector] public int ColumnInGrid;

        private void Awake()
        {
            PopNumbers = new();
        }

        public Piece Init(NumbersAndColorsSO so, int row, int column)
        {
            PieceNo = so.Number;
            RowInGrid = row;
            ColumnInGrid = column;

            return this;
        }

    }
}
