using System;
using System.Collections;

namespace Core.Scripts.Infrastructure.Loading
{
    public interface ISceneLoader
    {
        void LoadScene(string name, Action onLoaded = null);
    }
}