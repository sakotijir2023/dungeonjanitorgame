using System;
using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonJanitor.Input
{
    /// <summary>
    /// The only place that talks to the Input System. Exposes player input as
    /// simple values and events, and turns the debug toggle into a bus event.
    /// Player input is switched off while the tuning panel is open and once
    /// the shift has ended.
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
        const string ToolSlotName = "ToolSlot";
        const string CycleToolName = "CycleTool";
        const string ToggleTuningName = "ToggleTuningPanel";
        const string RestartRunName = "RestartRun";

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
        InputAction toolSlot;
        InputAction cycleTool;
        InputAction toggleTuning;
        InputAction restartRun;
        bool tuningOpen;
        bool runOver;

        public event Action JumpPressed;
        /// <summary>Number key shortcut; zero-based slot index.</summary>
        public event Action<int> ToolSlotPressed;
        /// <summary>D-pad left/right; -1 or +1.</summary>
        public event Action<int> ToolCyclePressed;

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
            toolSlot = playerMap.FindAction(ToolSlotName, true);
            cycleTool = playerMap.FindAction(CycleToolName, true);
            toggleTuning = debugMap.FindAction(ToggleTuningName, true);
            restartRun = debugMap.FindAction(RestartRunName, true);
        }

        void OnEnable()
        {
            jump.performed += OnJump;
            toolSlot.performed += OnToolSlot;
            cycleTool.performed += OnCycleTool;
            toggleTuning.performed += OnToggleTuning;
            restartRun.performed += OnRestartRun;
            EventBus<TuningPanelVisibilityChanged>.Subscribe(OnTuningPanelVisibility);
            EventBus<RunEnded>.Subscribe(OnRunEnded);
            playerMap.Enable();
            debugMap.Enable();
        }

        void OnDisable()
        {
            jump.performed -= OnJump;
            toolSlot.performed -= OnToolSlot;
            cycleTool.performed -= OnCycleTool;
            toggleTuning.performed -= OnToggleTuning;
            restartRun.performed -= OnRestartRun;
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnTuningPanelVisibility);
            EventBus<RunEnded>.Unsubscribe(OnRunEnded);
            playerMap.Disable();
            debugMap.Disable();
        }

        void OnJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();

        void OnToolSlot(InputAction.CallbackContext context)
        {
            // Bound to the digit keys; the control name is the digit ("1".."8").
            if (int.TryParse(context.control.name, out int digit))
                ToolSlotPressed?.Invoke(digit - 1);
        }

        void OnCycleTool(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (value != 0f)
                ToolCyclePressed?.Invoke(value > 0f ? 1 : -1);
        }

        void OnToggleTuning(InputAction.CallbackContext _) =>
            EventBus<TuningPanelToggleRequested>.Raise(new TuningPanelToggleRequested());

        void OnRestartRun(InputAction.CallbackContext _) =>
            EventBus<RestartRunRequested>.Raise(new RestartRunRequested());

        void OnTuningPanelVisibility(TuningPanelVisibilityChanged evt)
        {
            tuningOpen = evt.Visible;
            ApplyPlayerMap();
        }

        void OnRunEnded(RunEnded _)
        {
            runOver = true;
            ApplyPlayerMap();
        }

        void ApplyPlayerMap()
        {
            if (tuningOpen || runOver)
                playerMap.Disable();
            else
                playerMap.Enable();
        }
    }
}
