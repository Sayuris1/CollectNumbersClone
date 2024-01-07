using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Rootcraft.SceneManagement
{
    public class InitalLoader : Singleton<InitalLoader>
    {
        [SerializeField] private AssetReference _persistentManagersScene;
        [SerializeField] private AssetReference _mainScene;

        protected override void Awake()
        {
            base.Awake();
            // Load MainScene after fully loading PersistentManagers
            LoadSceneAsync(_persistentManagersScene,
                (_) => LoadSceneAsync(_mainScene,
                    (handle) =>
                    {
                        SceneManager.SetActiveScene(handle.Result.Scene);
                        UnloadInitalScene();
                    }));
        }

        private void LoadSceneAsync(AssetReference scene, Action<AsyncOperationHandle<SceneInstance>> OnComplete = null)
        {
            AsyncOperationHandle<SceneInstance> managerLoadHandler = scene.LoadSceneAsync(LoadSceneMode.Additive);
            managerLoadHandler.Completed += (AsyncOperationHandle<SceneInstance> handle) => IsLoadSucceeded(handle, scene.ToString(), OnComplete);;
        }

        private void IsLoadSucceeded(AsyncOperationHandle<SceneInstance> handle, string sceneName, Action<AsyncOperationHandle<SceneInstance>> OnComplete = null)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
                Debug.LogError($"Scene loading failed, {sceneName} stays unloaded.");
            else
                OnComplete?.Invoke(handle);
        }

        private void UnloadInitalScene()
        {
            SceneManager.UnloadSceneAsync(0).completed +=
                (UnityEngine.AsyncOperation handle) =>
                {
                    if(!handle.isDone)
                        Debug.LogError($"Scene unloading failed, initalScene stays loaded.");
                };
        }
    }
}