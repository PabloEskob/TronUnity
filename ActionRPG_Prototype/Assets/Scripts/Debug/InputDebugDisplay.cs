// InputDebugDisplay.cs (опционально, для отладки)

using UnityEngine;

namespace Debug
{
    public class InputDebugDisplay : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        void Start()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();

            // Подписка на события для отладки
            _inputActions.Player.Move.performed += ctx =>
                UnityEngine.Debug.Log($"Move: {ctx.ReadValue<Vector2>()}");

            _inputActions.Player.Run.performed += _ =>
                UnityEngine.Debug.Log("Run: Started");

            _inputActions.Player.Run.canceled += _ =>
                UnityEngine.Debug.Log("Run: Stopped");

            _inputActions.Player.Dodge.performed += _ =>
                UnityEngine.Debug.Log("Dodge: Triggered");
        }

        void OnDestroy()
        {
            _inputActions?.Disable();
        }
    }
}