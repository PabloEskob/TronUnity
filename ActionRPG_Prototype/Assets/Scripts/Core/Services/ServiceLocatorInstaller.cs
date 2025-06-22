using UnityEngine;

namespace Core.Services
{
    public class ServiceLocatorInstaller : MonoBehaviour
    {
        [SerializeField] private bool dontDestroy = true;

        private void Awake()
        {
            if (ServiceLocator.HasInstance)
            {
                UnityEngine.Debug.LogWarning("ServiceLocator already exists. Destroying duplicate installer.");
                Destroy(gameObject);
                return;
            }

            ServiceLocator.CreateInstance();

            if (dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }

            // Здесь можно зарегистрировать конкретные сервисы:
            // ServiceLocator.Instance.RegisterService<ISomeService>(new SomeService());
        }
    }
}