using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using static Rootcraft.CollectNumber.Extensions;
using UnityEngine;
using System.Collections;
using System.Linq;
using Rootcraft.CollectNumber.UI;

namespace Rootcraft.CollectNumber.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        [HideInInspector] public int _remainigMoves;

        private ResourceManager _rmInstance;
        private GridManager _gmInstance;

        private LevelsSO _currentLevel;
        private int _currentLevelIndex;

        protected override void Awake()
        {
            base.Awake();

            _currentLevelIndex = 0;
        }

        private void Start()
        {
            _rmInstance = ResourceManager.Instance;
            _gmInstance = GridManager.Instance;

            // Wait for level data
            _rmInstance.LevelsLoadHandle.Completed += (_) =>
            {
                _rmInstance.NumbersAndColorsLoadHandle.Completed += (_) =>
                {
                    SetNextLevel();
                    _gmInstance.CreateGrid(_currentLevel);
                };
            };
        }

        private void SetNextLevel()
        {
            _currentLevel = _rmInstance.GetLevel($"level{++_currentLevelIndex}");

            _remainigMoves = _currentLevel.RemainigMoves;

            GridManager.Instance.SetNextLevel(_currentLevel.Row, _currentLevel.Column);
            MainUIManager.Instance.SetNextLevel(_currentLevel.LevelRequiredNumbers, _remainigMoves.ToString());
            GameClient.Instance.SetCameraPosZoomFromGrid(_currentLevel.Row, _currentLevel.Column);
        }

        public void PieceIncreased()
        {
            _remainigMoves--;

            MainUIManager.Instance.UpdateRemainig(_remainigMoves.ToString());

            if(_remainigMoves <= 0)
                Debug.Log("gameOVer");
        }

        public List<string> GetLevelIgnoreList(int x, int y)
        {
            List<string> levelIgnoreList = new();
            foreach (PlacedNumber number in _currentLevel.PlacedNumberList)
            {
                if(number.PlacedNumberX != x && number.PlacedNumberY != y)
                    continue;

                levelIgnoreList.Add(number.PlacedNumberAndColor.Number.ToString());
            }
            
            return levelIgnoreList;
        }
    }
}