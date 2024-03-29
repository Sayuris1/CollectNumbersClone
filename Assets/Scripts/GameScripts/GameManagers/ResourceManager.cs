using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Rootcraft.CollectNumber.Resource
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public int NumbersAndColorsLenght {get {return _numbersAndColorsDict.Count;}}
        public int LevelsLenght {get {return _levelsDict.Count;}}
        public AsyncOperationHandle NumbersAndColorsLoadHandle;
        public AsyncOperationHandle LevelsLoadHandle;
        
        private Dictionary<string, NumbersAndColorsSO> _numbersAndColorsDict;
        private Dictionary<string, LevelsSO> _levelsDict;
        private List<string> _numbersAndColorsKeyList;

        protected override void Awake()
        {
            base.Awake();

            LoadNumbersAndColors();
            LoadLevels();
        }

        #region Loaders
        private void LoadNumbersAndColors()
        {
            var handle = LoadAssets<NumbersAndColorsSO>("NumbersAndColors");
            NumbersAndColorsLoadHandle = handle;

            handle.Completed += (h) =>
            {
                _numbersAndColorsDict = OnCompleteLoad<NumbersAndColorsSO>(handle, (d, i) =>
                {
                    d.Add(i.name, i);
                });

                _numbersAndColorsKeyList = new(_numbersAndColorsDict.Keys);
            };
        }

        private void LoadLevels()
        {
            var handle = LoadAssets<LevelsSO>("Levels");
            LevelsLoadHandle = handle;

            handle.Completed += (h) =>
            {
                _levelsDict = OnCompleteLoad<LevelsSO>(handle, (d, i) =>
                {
                    d.Add(i.name, i);
                });
            };
        }
        #endregion

        #region Getters
        public NumbersAndColorsSO GetNumberAndColor(string key)
        {
            return _numbersAndColorsDict[key];
        }

        public NumbersAndColorsSO GetRandomNumberAndColor(List<string> ignoreList)
        {
            string randomKey = _numbersAndColorsKeyList.Except(ignoreList).ElementAt(Random.Range(0, _numbersAndColorsKeyList.Count - ignoreList.Count));

            return _numbersAndColorsDict[randomKey];
        }

        public LevelsSO GetLevel(string key)
        {
            return _levelsDict[key];
        }
        #endregion

        #region AsyncHelpers
        private AsyncOperationHandle<IList<T>> LoadAssets<T>(string key, Action<T> callback = null)
        {
            return Addressables.LoadAssetsAsync<T>(key, callback, true);
        }

        private Dictionary<string, T> OnCompleteLoad<T>(AsyncOperationHandle<IList<T>> handle, Action<Dictionary<string, T>, T> handleDictionary)
        {
            Dictionary<string, T> dict = new();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                    handleDictionary(dict, item);
            }

            else
                Debug.LogErrorFormat($"<color=red>[RM]</color> Could not load {typeof(T).FullName}.");

            return dict;
        }
        #endregion
    }
}