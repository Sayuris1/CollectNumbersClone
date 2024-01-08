using System;
using System.Collections.Generic;
using Rootcraft.CollectNumber.Level;

namespace Rootcraft.CollectNumber
{
    public static class Extensions
    {
        public static List<string> AddChainedPieceIfPop(this List<string> ignoreList, Level.ChainedPiece chainedPiece)
        {
            if(chainedPiece.ChainedCount >= 1)
                ignoreList.Add(chainedPiece.PieceNo.ToString());

            return ignoreList;
        }

        public static EmptyByX[] Init(this EmptyByX[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].StartY = int.MaxValue;
            }

            return array;
        }
    }
}