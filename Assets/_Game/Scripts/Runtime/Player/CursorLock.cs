using DungeonJanitor.Core;
using UnityEngine;

namespace DungeonJanitor.Player
{
    /// <summary>Locks and hides the cursor during play; frees it while the tuning panel is open.</summary>
    public sealed class CursorLock : MonoBehaviour
    {
        bool panelOpen;

        void OnEnable() => EventBus<TuningPanelVisibilityChanged>.Subscribe(OnPanelVisibility);

        void OnDisable()
        {
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnPanelVisibility);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Start() => Apply();

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                Apply();
        }

        void OnPanelVisibility(TuningPanelVisibilityChanged evt)
        {
            panelOpen = evt.Visible;
            Apply();
        }

        void Apply()
        {
            Cursor.lockState = panelOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = panelOpen;
        }
    }
}
