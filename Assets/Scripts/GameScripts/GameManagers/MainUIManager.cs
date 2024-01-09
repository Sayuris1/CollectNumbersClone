using System.Collections.Generic;
using Rootcraft.CollectNumber.InputSystem;
using Rootcraft.CollectNumber.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rootcraft.CollectNumber.UI
{
    public class MainUIManager : Singleton<MainUIManager>
    {
        public GameObject RequiredNumberUIPrefab;
        public Transform RequiredNumberGroup;

        [SerializeField] private TMP_Text _remainigTmp;
        [SerializeField] private GameObject _retryGO;
        [SerializeField] private GameObject _nextLevelGO;
        private Dictionary<int, TMP_Text> _requiredNumberGraphicDic;

        protected override void Awake()
        {
            base.Awake();
        }

        public void SetNextLevel(List<LevelRequiredNumber> requiredNumbers, string remainig)
        {
            InputManager.Instance.EnableGameInput();

            _retryGO.SetActive(false);
            _nextLevelGO.SetActive(false);

            _requiredNumberGraphicDic = new();

            foreach (LevelRequiredNumber requiredNumber in requiredNumbers)
            {
                GameObject newGO = Instantiate(RequiredNumberUIPrefab);
                newGO.transform.SetParent(RequiredNumberGroup, false);

                TMP_Text tmp = newGO.GetComponentInChildren<TMP_Text>();
                tmp.text = requiredNumber.RequiredCount.ToString();
                newGO.GetComponentInChildren<Image>().color = requiredNumber.PlacedNumberAndColor.Color;

                _requiredNumberGraphicDic.Add(requiredNumber.PlacedNumberAndColor.Number, tmp);

                UpdateRemainig(remainig);
            }
        }

        #region UpdateGraphic
        public void UpdateRemainig(string remainig)
        {
            _remainigTmp.text = remainig;
        }

        public void UpdateRemainigRequired(int no, string text)
        {
            _requiredNumberGraphicDic[no].text = text;
        }

        public void UpdateGoal(string remainig)
        {
            _remainigTmp.text = remainig;
        }
        
        public void EnableRetry()
        {
            _retryGO.SetActive(true);
            InputManager.Instance.DisableGameInput();
        }

        public void EnableNextLevel()
        {
            _nextLevelGO.SetActive(true);
            InputManager.Instance.DisableGameInput();
        }
        #endregion

        #region Input
        public void NextLevelPressed()
        {
            foreach (Transform child in RequiredNumberGroup.transform)
                Destroy(child.gameObject);

            LevelManager.Instance.SetNextLevel();
        }

        public void RetryPressed()
        {
            foreach (Transform child in RequiredNumberGroup.transform)
                Destroy(child.gameObject);

            LevelManager.Instance.SetLevel();
        }
        #endregion
    }
}