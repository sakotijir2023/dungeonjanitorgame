using System;
using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>What the player picked when the wheel closed. Slot -1 = nothing.</summary>
    public readonly struct WheelPick
    {
        public readonly int Slot;
        /// <summary>Folder option index, or -1 when the slot itself was picked.</summary>
        public readonly int Option;

        public WheelPick(int slot, int option)
        {
            Slot = slot;
            Option = option;
        }

        public bool IsNone => Slot < 0;
        public static WheelPick None => new WheelPick(-1, -1);
    }

    /// <summary>
    /// Pure hover and folder logic for the tool wheel, so it can be tested
    /// without input or UI. Resting on a folder opens it; inside a folder,
    /// wedge 0 is "Back" and sits where the folder slot was. A slot must be
    /// newly entered before resting on it can open or close a folder, so the
    /// wheel never flips back and forth on its own.
    /// </summary>
    public sealed class WheelNavigator
    {
        readonly int mainCount;
        readonly Func<int, int> folderOptionCount;

        bool armed;
        float restTime;

        /// <param name="folderOptionCount">Options in a main slot's folder; 0 if the slot is not a folder.</param>
        public WheelNavigator(int mainCount, Func<int, int> folderOptionCount)
        {
            this.mainCount = mainCount;
            this.folderOptionCount = folderOptionCount;
            Reset();
        }

        /// <summary>Open folder's main slot, or -1 on the main ring.</summary>
        public int FolderSlot { get; private set; }
        /// <summary>Hovered wedge on the current ring, or -1.</summary>
        public int Hovered { get; private set; }
        public bool InFolder => FolderSlot >= 0;
        public int RingCount => InFolder ? folderOptionCount(FolderSlot) + 1 : mainCount;
        public float RingFirstAngle => InFolder ? WheelMath.SlotAngle(FolderSlot, mainCount) : 0f;
        public bool HoveringBack => InFolder && Hovered == 0;
        public bool HoveringFolder => !InFolder && Hovered >= 0 && folderOptionCount(Hovered) > 0;

        public void Reset()
        {
            FolderSlot = -1;
            Hovered = -1;
            armed = false;
            restTime = 0f;
        }

        /// <summary>
        /// Feed one frame. Pass null when the player is not pointing anywhere
        /// (the last hover is kept). Returns true if the ring changed.
        /// </summary>
        public bool Update(Vector2? direction, float deltaTime, float restDelay)
        {
            if (direction.HasValue)
            {
                int index = WheelMath.IndexFromDirection(direction.Value, RingCount, RingFirstAngle);
                if (index != Hovered)
                {
                    Hovered = index;
                    armed = true;
                    restTime = 0f;
                }
            }

            if (!armed || !(HoveringFolder || HoveringBack))
                return false;

            restTime += deltaTime;
            if (restTime < restDelay)
                return false;

            FolderSlot = InFolder ? -1 : Hovered;
            armed = false;
            restTime = 0f;
            Hovered = direction.HasValue
                ? WheelMath.IndexFromDirection(direction.Value, RingCount, RingFirstAngle)
                : (InFolder ? 0 : -1);
            return true;
        }

        public WheelPick Pick()
        {
            if (Hovered < 0 || HoveringBack)
                return WheelPick.None;
            return InFolder ? new WheelPick(FolderSlot, Hovered - 1) : new WheelPick(Hovered, -1);
        }
    }
}
