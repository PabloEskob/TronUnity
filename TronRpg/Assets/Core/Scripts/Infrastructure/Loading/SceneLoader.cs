using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scripts.Infrastructure.Loading
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void Load(string sceneName, Action onLoaded = null)
        {
            Debug.Log($"SceneLoader.Load called with scene: {sceneName}");
        
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                return;
            }
        
            if (onLoaded != null)
            {
                void OnSceneLoaded(Scene scene, LoadSceneMode mode)
                {
                    if (scene.name == sceneName)
                    {
                        SceneManager.sceneLoaded -= OnSceneLoaded;
                        Debug.Log($"Scene {sceneName} loaded via event, calling onLoaded");
                        onLoaded?.Invoke();
                    }
                }
            
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        
            Debug.Log($"Starting to load scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        public IEnumerator LoadScene(string sceneName, Action onLoaded = null)
        {
            Debug.Log($"LoadScene coroutine started for: {sceneName}");
    
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                Debug.Log($"Scene {sceneName} already active");
                onLoaded?.Invoke();
                yield break;
            }
    
            Debug.Log($"Loading scene {sceneName} async...");
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(sceneName);
    
            if (waitNextScene == null)
            {
                Debug.LogError($"LoadSceneAsync returned null for scene: {sceneName}");
                yield break;
            }
    
            // ВАЖНО: Убедитесь, что сцена может активироваться
            waitNextScene.allowSceneActivation = true;
    
            // Используйте progress >= 0.9f вместо isDone
            while (waitNextScene.progress < 0.9f)
            {
                Debug.Log($"Loading progress: {waitNextScene.progress}");
                yield return null;
            }
    
            // Дождитесь полного завершения
            while (!waitNextScene.isDone)
            {
                Debug.Log($"Waiting for scene activation... progress: {waitNextScene.progress}");
                yield return null;
            }

            Debug.Log($"Scene {sceneName} loaded successfully, calling onLoaded");
            onLoaded?.Invoke();
            Debug.Log($"onLoaded called");
        }
    }
}