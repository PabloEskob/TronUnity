using Core.Input;
using Core.Services;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Service References")] [SerializeField]
    private GameObject _serviceLocatorPrefab;

    [SerializeField] private GameObject _inputManagerPrefab;

    private void Awake()
    {
        // Убедимся что ServiceLocator существует
        if (ServiceLocator.Instance == null && _serviceLocatorPrefab != null)
        {
            Instantiate(_serviceLocatorPrefab);
        }

        // Убедимся что InputManager существует
        var inputManager = FindAnyObjectByType<InputManager>();
        if (inputManager == null && _inputManagerPrefab != null)
        {
            Instantiate(_inputManagerPrefab);
        }
    }
}