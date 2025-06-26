using System.Collections.Generic;
using Core.Scripts.Character;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Aim
{
    public class SimplePlayerAimController : MonoBehaviour, IInputAxisOwner
    {
        public enum CouplingMode
        {
            Coupled,
            CoupledWhenMoving,
            Decoupled
        }
        
        public CouplingMode PlayerRotation;
        public float RotationDamping = 0.2f;
        public InputAxis HorizontalLook = new() { Range = new Vector2(-180, 180), Wrap = true, Recentering = InputAxis.RecenteringSettings.Default };
        public InputAxis VerticalLook = new() { Range = new Vector2(-70, 70), Recentering = InputAxis.RecenteringSettings.Default };

        private HeroMove _controller;
        private Transform _controllerTransform;
        private Quaternion _desiredWorldRotation;
        
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref HorizontalLook, Name = "Horizontal Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new() { DrivenAxis = () => ref VerticalLook, Name = "Vertical Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
        }

        void OnValidate()
        {
            HorizontalLook.Validate();
            VerticalLook.Range.x = Mathf.Clamp(VerticalLook.Range.x, -90, 90);
            VerticalLook.Range.y = Mathf.Clamp(VerticalLook.Range.y, -90, 90);
            VerticalLook.Validate();
        }

        void OnEnable()
        {
            _controller = GetComponentInParent<HeroMove>();
            if (_controller == null)
                Debug.LogError("SimplePlayerController not found on parent object");
            else
            {
                _controller.PreUpdate -= UpdatePlayerRotation;
                _controller.PreUpdate += UpdatePlayerRotation;
                _controller.PostUpdate -= PostUpdate;
                _controller.PostUpdate += PostUpdate;
                _controllerTransform = _controller.transform;
            }
        }

        void OnDisable()
        {
            if (_controller != null)
            {
                _controller.PreUpdate -= UpdatePlayerRotation;
                _controller.PostUpdate -= PostUpdate;
                _controllerTransform = null;
            }
        }

        public void RecenterPlayer(float damping = 0)
        {
            if (_controllerTransform == null)
                return;
            
            var rot = transform.localRotation.eulerAngles;
            rot.y = NormalizeAngle(rot.y);
            var delta = rot.y;
            delta = Damper.Damp(delta, damping, Time.deltaTime);
            
            _controllerTransform.rotation = Quaternion.AngleAxis(
                delta, _controllerTransform.up) * _controllerTransform.rotation;
            
            HorizontalLook.Value -= delta;
            rot.y -= delta;
            transform.localRotation = Quaternion.Euler(rot);
        }
        
        public void SetLookDirection(Vector3 worldspaceDirection)
        {
            if (_controllerTransform == null)
                return;
            var rot = (Quaternion.Inverse(_controllerTransform.rotation)
                       * Quaternion.LookRotation(worldspaceDirection, _controllerTransform.up)).eulerAngles;
            HorizontalLook.Value = HorizontalLook.ClampValue(rot.y);
            VerticalLook.Value = VerticalLook.ClampValue(NormalizeAngle(rot.x));
        }
        
        void UpdatePlayerRotation()
        {
            var t = transform;
            t.localRotation = Quaternion.Euler(VerticalLook.Value, HorizontalLook.Value, 0);
            _desiredWorldRotation = t.rotation;
            switch (PlayerRotation)
            {
                case CouplingMode.Coupled:
                {
                    _controller.SetStrafeMode(true);
                    RecenterPlayer();
                    break;
                }
                case CouplingMode.CoupledWhenMoving:
                {
                    _controller.SetStrafeMode(true);
                    if (_controller.IsMoving)
                        RecenterPlayer(RotationDamping);
                    break;
                }
                case CouplingMode.Decoupled:
                {
                    _controller.SetStrafeMode(false);
                    break;
                }
            }

            VerticalLook.UpdateRecentering(Time.deltaTime, VerticalLook.TrackValueChange());
            HorizontalLook.UpdateRecentering(Time.deltaTime, HorizontalLook.TrackValueChange());
        }
        
        void PostUpdate(Vector3 vel, float speed)
        {
            if (PlayerRotation == CouplingMode.Decoupled)
            {
                transform.rotation = _desiredWorldRotation;
                var delta = (Quaternion.Inverse(_controllerTransform.rotation) * _desiredWorldRotation).eulerAngles;
                VerticalLook.Value = NormalizeAngle(delta.x);
                HorizontalLook.Value = NormalizeAngle(delta.y);
            }
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }
    }
}