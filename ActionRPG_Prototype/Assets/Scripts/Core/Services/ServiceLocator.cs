using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    private static bool _isShuttingDown;

    private readonly Dictionary<Type, object> _services = new();

    public static bool HasInstance => !_isShuttingDown && _instance != null;

    public static ServiceLocator Instance
    {
        get
        {
            if (!_isShuttingDown && _instance == null)
                UnityEngine.Debug.LogError("ServiceLocator not initialized! Call CreateInstance() early.");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    public static void CreateInstance()
    {
        if (_instance != null) return;

        var existing = FindFirstObjectByType<ServiceLocator>();
        if (existing != null)
        {
            _instance = existing;
            return;
        }

        var go = new GameObject("[ServiceLocator]");
        _instance = go.AddComponent<ServiceLocator>();
        DontDestroyOnLoad(go);
    }

    public void RegisterService<T>(T service) where T : class
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            UnityEngine.Debug.LogWarning($"Service {type.Name} already registered.");
            return;
        }

        _services[type] = service;
    }

    public T GetService<T>() where T : class
    {
        var type = typeof(T);
        return _services.TryGetValue(type, out var service) ? (T)service : null;
    }

    public void UnregisterService<T>() where T : class
    {
        var type = typeof(T);
        _services.Remove(type);
    }
}