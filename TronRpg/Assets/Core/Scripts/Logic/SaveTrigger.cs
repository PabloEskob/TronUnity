using System;
using Core.Scripts.Services.SaveLoad;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Logic
{
    public class SaveTrigger : MonoBehaviour
    {
        private ISaveLoadService _saveLoadService;

        public BoxCollider Collider;

        [Inject]
        public void Construct(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        private void OnTriggerEnter(Collider other)
        {
            _saveLoadService.SaveProgress();
            Debug.Log("Save Complete");
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (!Collider)
                return;
            Gizmos.color = new Color(30, 200, 30, 130);
            Gizmos.DrawCube(transform.position + Collider.center, Collider.size);
        }
    }
}