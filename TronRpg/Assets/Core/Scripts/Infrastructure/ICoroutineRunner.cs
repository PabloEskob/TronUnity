using System.Collections;
using UnityEngine;

namespace Core.Scripts.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}