using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DungeonJanitor.Tuning
{
    /// <summary>Keeps the gamepad-selected control visible inside a scroll view.</summary>
    [RequireComponent(typeof(ScrollRect))]
    public sealed class ScrollToSelected : MonoBehaviour
    {
        ScrollRect scrollRect;
        GameObject lastSelected;

        void Awake() => scrollRect = GetComponent<ScrollRect>();

        void LateUpdate()
        {
            GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            if (selected == lastSelected)
                return;
            lastSelected = selected;
            if (selected == null || !selected.transform.IsChildOf(scrollRect.content))
                return;

            RectTransform viewport = scrollRect.viewport;
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, selected.transform);
            Rect view = viewport.rect;
            Vector2 position = scrollRect.content.anchoredPosition;

            if (bounds.max.y > view.yMax)
                position.y -= bounds.max.y - view.yMax;
            else if (bounds.min.y < view.yMin)
                position.y += view.yMin - bounds.min.y;

            scrollRect.content.anchoredPosition = position;
        }
    }
}
