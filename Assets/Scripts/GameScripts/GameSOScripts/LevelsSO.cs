using System.Collections.Generic;
using Rootcraft.CollectNumber.Level;
using UnityEngine;

[CreateAssetMenu]
public class LevelsSO : ScriptableObject
{
    public int Row;
    public int Column;
    public int RemainigMoves;
    public List<PlacedNumber> PlacedNumberList;
    public List<LevelRequiredNumber> LevelRequiredNumbers;
}
