using System.Collections.Generic;
using Rootcraft.CollectNumber.Resource;
using static Rootcraft.CollectNumber.Extensions;
using UnityEngine;
using System.Collections;
using System.Linq;
using Rootcraft.CollectNumber.UI;
using Unity.Mathematics;

namespace Rootcraft.CollectNumber.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        private int _remainigMoves;
        private Dictionary<int, int> _remainingRequired;

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
                };
            };
        }

        #region LevelControls
        public void SetLevel()
        {
            _remainigMoves = _currentLevel.RemainigMoves;

            _remainingRequired = new();
            foreach (LevelRequiredNumber item in _currentLevel.LevelRequiredNumbers)
                _remainingRequired.Add(item.PlacedNumberAndColor.Number, item.RequiredCount);

            GridManager.Instance.SetNextLevel(_currentLevel.Row, _currentLevel.Column);
            MainUIManager.Instance.SetNextLevel(_currentLevel.LevelRequiredNumbers, _remainigMoves.ToString());
            GameClient.Instance.SetCameraPosZoomFromGrid(_currentLevel.Row, _currentLevel.Column);

            _gmInstance.CreateGrid(_currentLevel);
        }

        public void SetNextLevel()
        {
            _currentLevelIndex = math.min(++_currentLevelIndex, _rmInstance.LevelsLenght);
            _currentLevel = _rmInstance.GetLevel($"level{_currentLevelIndex}");

            SetLevel();
        }
        #endregion

        #region LevelStatus
        public void PieceIncreased()
        {
            _remainigMoves--;

            MainUIManager.Instance.UpdateRemainig(_remainigMoves.ToString());

            if(_remainigMoves <= 0)
                MainUIManager.Instance.EnableRetry();
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

        public void Poped(int no)
        {
            if(!_remainingRequired.ContainsKey(no))
                return;

            int req = math.max(--_remainingRequired[no], 0);
            MainUIManager.Instance.UpdateRemainigRequired(no, req.ToString());

            foreach (KeyValuePair<int, int> requ in _remainingRequired)
            {
                if(requ.Value != 0)
                    break;
                
                MainUIManager.Instance.EnableNextLevel();
            }
        }
        #endregion
    }
}