using System;

namespace Rootcraft.CollectNumber.Level
{
    public struct ChainedPiece
    {
        public int PieceNo;
        public int ChainedCount;
    }

    public struct PiecePos
    {
        public int x;
        public int y;
    }

    public struct EmptyByX
    {
        public int StartY;
        public int EmptyCount;
    }

    [Serializable]
    public struct PlacedNumber
    {
        public NumbersAndColorsSO PlacedNumberAndColor;
        public int PlacedNumberX;
        public int PlacedNumberY;
    }
}