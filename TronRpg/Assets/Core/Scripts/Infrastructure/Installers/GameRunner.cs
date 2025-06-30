using System;
using UnityEngine;

namespace Core.Scripts.Infrastructure.Installers
{
    public class GameRunner : MonoBehaviour
    {
        public BootstrapInstaller BootstrapInstallerPrefab;

        private void Awake()
        {
            BootstrapInstaller bootstrapInstaller = FindAnyObjectByType<BootstrapInstaller>();
            if (!bootstrapInstaller)
            {
                Instantiate(BootstrapInstallerPrefab);
            }
        }
    }
}