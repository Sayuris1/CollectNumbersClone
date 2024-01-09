using System.Collections.Generic;
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
        private Dictionary<Color, TMP_Text> _requiredNumberGraphicDic;

        protected override void Awake()
        {
            base.Awake();

            _requiredNumberGraphicDic = new();
        }

        public void SetNextLevel(List<LevelRequiredNumber> requiredNumbers, string remainig)
        {
            foreach (LevelRequiredNumber requiredNumber in requiredNumbers)
            {
                GameObject newGO = Instantiate(RequiredNumberUIPrefab);
                newGO.transform.SetParent(RequiredNumberGroup, false);

                TMP_Text tmp = newGO.GetComponentInChildren<TMP_Text>();
                tmp.text = requiredNumber.RequiredCount.ToString();
                newGO.GetComponentInChildren<Image>().color = requiredNumber.PlacedNumberAndColor.Color;

                _requiredNumberGraphicDic.Add(requiredNumber.PlacedNumberAndColor.Color, tmp);

                UpdateRemainig(remainig);
            }
        }

        public void UpdateRemainig(string remainig)
        {
            _remainigTmp.text = remainig;
        }
    }
}