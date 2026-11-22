using System;
using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonJanitor.Input
{
    /// <summary>
    /// The only place that talks to the Input System. Exposes player input as
    /// simple values and events, and turns the debug toggle into a bus event.
    /// Player input is switched off while the tuning panel is open.
    /// </summary>
    public sealed class InputReader : MonoBehaviour
    {
        const string PlayerMapName = "Player";
        const string DebugMapName = "Debug";
        const string MoveName = "Move";
        const string LookName = "Look";
        const string JumpName = "Jump";
        const string SprintName = "Sprint";
        const string UseName = "Use";
        const string SecondaryName = "Secondary";
        const string ToolWheelName = "ToolWheel";
        const string ToggleTuningName = "ToggleTuningPanel";

        [SerializeField] InputActionAsset actions;

        InputActionMap playerMap;
        InputActionMap debugMap;
        InputAction move;
        InputAction look;
        InputAction jump;
        InputAction sprint;
        InputAction use;
        InputAction secondary;
        InputAction toolWheel;
        InputAction toggleTuning;

        public event Action JumpPressed;

        public Vector2 Move => move.ReadValue<Vector2>();
        public Vector2 Look => look.ReadValue<Vector2>();
        /// <summary>True when the current look input comes from a mouse (pixels), false for a stick (-1..1).</summary>
        public bool LookIsMouse => look.activeControl?.device is Pointer;
        public bool SprintHeld => sprint.IsPressed();
        public bool UseHeld => use.IsPressed();
        public bool SecondaryHeld => secondary.IsPressed();
        public bool ToolWheelHeld => toolWheel.IsPressed();

        void Awake()
        {
            if (actions == null)
            {
                Debug.LogError($"{nameof(InputReader)} on {name} has no Input Actions asset assigned.", this);
                enabled = false;
                return;
            }
            playerMap = actions.FindActionMap(PlayerMapName, true);
            debugMap = actions.FindActionMap(DebugMapName, true);
            move = playerMap.FindAction(MoveName, true);
            look = playerMap.FindAction(LookName, true);
            jump = playerMap.FindAction(JumpName, true);
            sprint = playerMap.FindAction(SprintName, true);
            use = playerMap.FindAction(UseName, true);
            secondary = playerMap.FindAction(SecondaryName, true);
            toolWheel = playerMap.FindAction(ToolWheelName, true);
            toggleTuning = debugMap.FindAction(ToggleTuningName, true);
        }

        void OnEnable()
        {
            jump.performed += OnJump;
            toggleTuning.performed += OnToggleTuning;
            EventBus<TuningPanelVisibilityChanged>.Subscribe(OnTuningPanelVisibility);
            playerMap.Enable();
            debugMap.Enable();
        }

        void OnDisable()
        {
            jump.performed -= OnJump;
            toggleTuning.performed -= OnToggleTuning;
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnTuningPanelVisibility);
            playerMap.Disable();
            debugMap.Disable();
        }

        void OnJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();

        void OnToggleTuning(InputAction.CallbackContext _) =>
            EventBus<TuningPanelToggleRequested>.Raise(new TuningPanelToggleRequested());

        void OnTuningPanelVisibility(TuningPanelVisibilityChanged evt)
        {
            if (evt.Visible)
                playerMap.Disable();
            else
                playerMap.Enable();
        }
    }
}
