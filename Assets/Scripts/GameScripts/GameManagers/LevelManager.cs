using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using static Rootcraft.CollectNumber.Extensions;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Rootcraft.CollectNumber.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
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
                SetNextLevel();
                _rmInstance.NumbersAndColorsLoadHandle.Completed += (_) => _gmInstance.CreateGrid(_currentLevel);
            };
        }

        private void SetNextLevel()
        {
            _currentLevel = _rmInstance.GetLevel($"level{++_currentLevelIndex}");

            GridManager.Instance.SetNextLevel(_currentLevel.Row, _currentLevel.Column);
            GameClient.Instance.SetCameraPosZoomFromGrid(_currentLevel.Row, _currentLevel.Column);
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