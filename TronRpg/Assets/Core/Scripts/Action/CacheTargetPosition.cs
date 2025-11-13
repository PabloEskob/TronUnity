using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

namespace Core.Scripts.Action
{
    public class CacheTargetPosition : Opsive.BehaviorDesigner.Runtime.Tasks.Actions.Action
    {
        [Tooltip("Сюда запишем позицию")] 
        public SharedVariable<Vector3> OutPosition;

        [Tooltip("Целевой трансформ")]
        public SharedVariable<GameObject> Target;

        private GameObject _targetGameObject;
        private Transform _targetTransform;


        public override void OnStart()
        {
            _targetGameObject = Target?.Value;
            _targetTransform = _targetGameObject?.transform;
        }

        public override TaskStatus OnUpdate()
        {
            var current = Target?.Value;
            if (current == null) return TaskStatus.Failure;
            if (!ReferenceEquals(current, _targetGameObject))
            {
                _targetGameObject = current;
                _targetTransform = current.transform;
            }

            OutPosition.Value = _targetTransform.position;
            return TaskStatus.Success;
        }
    }
}