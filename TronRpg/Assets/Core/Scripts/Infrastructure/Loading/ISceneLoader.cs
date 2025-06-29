using System;
using System.Collections;

namespace Core.Scripts.Infrastructure.Loading
{
    public interface ISceneLoader
    {
        void Load(string sceneName, Action onLoaded = null);
        IEnumerator LoadScene(string sceneName, Action onLoaded = null);
    }
}