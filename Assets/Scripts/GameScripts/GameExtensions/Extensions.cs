using System.Collections.Generic;

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
    }
}