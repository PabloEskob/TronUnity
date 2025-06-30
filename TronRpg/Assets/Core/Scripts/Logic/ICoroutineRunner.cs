using System.Collections;
using UnityEngine;

namespace Core.Scripts.Logic
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}