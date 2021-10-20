using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPSController
{
    [System.Serializable]
    public struct Inputs
    {
        [SerializeField] public Vector2 m_movement;
        [SerializeField] public Vector2 m_look;
        [SerializeField] public bool m_jump;
        [SerializeField] public bool m_crouch;
        [SerializeField] public bool m_primary;
        [SerializeField] public bool m_secondary;
        [SerializeField] public bool m_teleport;

        public Inputs(Vector2 movement, Vector2 look, bool jump, bool crouch, bool primary, bool secondary, bool teleport)
        {
            m_movement = movement;
            m_look = look;
            m_jump = jump;
            m_crouch = crouch;
            m_primary = primary;
            m_secondary = secondary;
            m_teleport = teleport;
        }

        public string GetString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private InputActionAsset m_playerInput;

        private InputAction m_movementAction;
        private InputAction m_jumpAction;
        private InputAction m_crouchAction;
        private InputAction m_primaryAction;
        private InputAction m_secondaryAction;
        private InputAction m_teleportAction;

        public System.Action teleport;

        private Inputs m_inputs;

        private void Awake()
        {
            InputActionMap actionMap = m_playerInput.FindActionMap("Default");

            m_movementAction = actionMap.FindAction("Move");
            m_movementAction.performed += onMovementChanged;
            m_movementAction.canceled += onMovementChanged;
            m_movementAction.Enable();

            m_jumpAction = actionMap.FindAction("Jump");
            m_jumpAction.performed += onJumpChanged;
            m_jumpAction.canceled += onJumpChanged;
            m_jumpAction.Enable();

            m_crouchAction = actionMap.FindAction("Crouch");
            m_crouchAction.performed += onCrouchChanged;
            m_crouchAction.canceled += onCrouchChanged;
            m_crouchAction.Enable();

            m_teleportAction = actionMap.FindAction("Teleport");
            m_teleportAction.performed += onTeleportChanged;

            m_primaryAction = actionMap.FindAction("Primary");
            m_secondaryAction = actionMap.FindAction("Secondary");
        }

        private void OnDestroy()
        {
            m_movementAction.performed -= onMovementChanged;
            m_movementAction.canceled -= onMovementChanged;
            m_movementAction.Disable();

            m_jumpAction.performed -= onJumpChanged;
            m_jumpAction.canceled -= onJumpChanged;
            m_jumpAction.Disable();

            m_crouchAction.performed -= onCrouchChanged;
            m_crouchAction.canceled -= onCrouchChanged;
            m_crouchAction.Disable();
        }

        public Inputs ReadInputs()
        {
            m_inputs.m_look = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());
            return m_inputs;
        }

        private void onMovementChanged(InputAction.CallbackContext context)
        {
            m_inputs.m_movement = context.ReadValue<Vector2>();
        }

        private void onJumpChanged(InputAction.CallbackContext context)
        {
            m_inputs.m_jump = context.ReadValue<float>() > 0;
        }

        private void onCrouchChanged(InputAction.CallbackContext context)
        {
            m_inputs.m_crouch = context.ReadValue<float>() > 0;
        }

        private void onTeleportChanged(InputAction.CallbackContext context) {
            Debug.Log(context.started);
            if(teleport != null) teleport();
        }
    }
}
