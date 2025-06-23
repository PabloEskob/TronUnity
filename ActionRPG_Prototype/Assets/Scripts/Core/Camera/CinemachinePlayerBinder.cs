using System;
using Core.Events;
using Core.Events.Messages;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Camera
{
    public sealed class CinemachinePlayerBinder : IStartable, IDisposable
    {
        readonly IEventBus _bus;
        readonly CinemachineCamera _cam;
        readonly CompositeDisposable _cd = new();

        [Inject]
        public CinemachinePlayerBinder(IEventBus bus, CinemachineCamera cam)
        {
            _bus = bus;
            _cam = cam;
        }

        public void Start()
        {
            Debug.Log("[Binder] Start");
            _bus.Receive<PlayerSpawned>()
                .Subscribe(e => Attach(e.Player.transform))
                .AddTo(_cd);
            
        }

        void Attach(Transform target)
        {
            Debug.Log("[Binder] Attach " + target.name);
            _cam.Follow = target;
            _cam.LookAt = target;
            
            var controller = _cam.GetComponent<GenshinCameraController>();
            Debug.Log("[Binder] controller = " + controller);   // ①
            controller.SetPlayer(target); 
        }

        public void Dispose() => _cd.Dispose();
    }
}