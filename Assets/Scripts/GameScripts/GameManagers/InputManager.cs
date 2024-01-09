using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rootcraft.CollectNumber.InputSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private GameObject _pointerPrefab;

        private Pointer _currentPointer;
        private PlayerInput _playerInput;

        private InputActionMap _gameInputActionMap;
        private InputActionMap _menuInputActionMap;

        private Dictionary<InputActionMap, bool> _inputActionState;

        protected override void Awake()
        {
            base.Awake();
            _currentPointer = Instantiate(_pointerPrefab).GetComponent<Pointer>();

            _playerInput = GetComponent<PlayerInput>();

            _gameInputActionMap = _playerInput.actions.FindActionMap("Game");
            _menuInputActionMap = _playerInput.actions.FindActionMap("Menu");

            _inputActionState = new Dictionary<InputActionMap, bool>()
            {
                {_gameInputActionMap , _gameInputActionMap.enabled},
                {_menuInputActionMap, _menuInputActionMap.enabled},
            };
        }

        // Simulate touchscreen in editor
        #if UNITY_EDITOR
            private void Update()
            {
                _playerInput.SwitchCurrentControlScheme(Touchscreen.current);
            }
        #endif

        #region MapControl
        public void EnableGameInput()
        {
            _gameInputActionMap.Enable();
        }

        public void DisableGameInput()
        {
            _gameInputActionMap.Disable();
        }
        #endregion

        #region Bindings
        public void OnPointerHold(InputAction.CallbackContext context)
        {
            _currentPointer.Hold(context);
        }

        public void OnPointerPress(InputAction.CallbackContext context)
        {
            if (context.ReadValue<Single>() == 1)
                _currentPointer.Press(context);
        }

        public void OnPointerRelease(InputAction.CallbackContext context)
        {
            if (context.ReadValue<Single>() == 0)
                _currentPointer.Release(context);
        }
        #endregion
    }
}