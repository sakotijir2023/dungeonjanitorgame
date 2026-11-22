using System.Collections.Generic;
using DungeonJanitor.Messes;
using DungeonJanitor.Tools.Wheel;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class ToolWheelTests
    {
        const int Slots = 8;
        const int FolderSlot = 7;
        const int FolderOptions = 4;
        const float Delay = 0.3f;
        const float Frame = 0.016f;

        static readonly Vector2 Up = Vector2.up;
        static readonly Vector2 Right = Vector2.right;
        static readonly Vector2 UpLeft = new Vector2(-1f, 1f);
        static readonly Vector2 Down = Vector2.down;

        static WheelNavigator NewNavigator() =>
            new WheelNavigator(Slots, slot => slot == FolderSlot ? FolderOptions : 0);

        static void Rest(WheelNavigator nav, Vector2? direction, float seconds)
        {
            for (float t = 0f; t < seconds; t += Frame)
                nav.Update(direction, Frame, Delay);
        }

        [Test]
        public void Directions_MapToClockwiseSlotsFromTop()
        {
            Assert.That(WheelMath.IndexFromDirection(Up, Slots, 0f), Is.EqualTo(0));
            Assert.That(WheelMath.IndexFromDirection(Right, Slots, 0f), Is.EqualTo(2));
            Assert.That(WheelMath.IndexFromDirection(Down, Slots, 0f), Is.EqualTo(4));
            Assert.That(WheelMath.IndexFromDirection(UpLeft, Slots, 0f), Is.EqualTo(7));
            // Just left of straight up still counts as the top slot.
            Assert.That(WheelMath.IndexFromDirection(new Vector2(-0.1f, 1f), Slots, 0f), Is.EqualTo(0));
        }

        [Test]
        public void DirectionFromIndex_RoundTrips()
        {
            for (int i = 0; i < Slots; i++)
            {
                Vector2 direction = WheelMath.DirectionFromIndex(i, Slots, 0f);
                Assert.That(WheelMath.IndexFromDirection(direction, Slots, 0f), Is.EqualTo(i));
            }
        }

        [Test]
        public void StickyIndex_HoldsSlotJustPastItsBorder()
        {
            // Slot 0 spans -22.5..22.5 deg. 27 deg is just inside slot 1.
            Vector2 justPast = WheelMath.DirectionFromIndex(0, 360, 27f);
            Assert.That(WheelMath.IndexFromDirection(justPast, Slots, 0f), Is.EqualTo(1));
            Assert.That(WheelMath.StickyIndex(justPast, Slots, 0f, 0, 8f), Is.EqualTo(0));
            // Well past the border, it moves on.
            Vector2 farPast = WheelMath.DirectionFromIndex(0, 360, 35f);
            Assert.That(WheelMath.StickyIndex(farPast, Slots, 0f, 0, 8f), Is.EqualTo(1));
            // Nothing hovered yet: no stickiness.
            Assert.That(WheelMath.StickyIndex(justPast, Slots, 0f, -1, 8f), Is.EqualTo(1));
        }

        [Test]
        public void ReleasingOnATool_PicksThatSlot()
        {
            WheelNavigator nav = NewNavigator();
            nav.Update(Right, Frame, Delay);
            WheelPick pick = nav.Pick();
            Assert.That(pick.Slot, Is.EqualTo(2));
            Assert.That(pick.Option, Is.EqualTo(-1));
        }

        [Test]
        public void NotPointing_PicksNothing()
        {
            WheelNavigator nav = NewNavigator();
            nav.Update(null, Frame, Delay);
            Assert.That(nav.Pick().IsNone, Is.True);
        }

        [Test]
        public void QuickReleaseOnFolder_PicksFolderItself()
        {
            WheelNavigator nav = NewNavigator();
            nav.Update(UpLeft, Frame, Delay);
            Assert.That(nav.InFolder, Is.False);
            WheelPick pick = nav.Pick();
            Assert.That(pick.Slot, Is.EqualTo(FolderSlot));
            Assert.That(pick.Option, Is.EqualTo(-1), "Option -1 means use the folder's remembered pick.");
        }

        [Test]
        public void RestingOnFolder_OpensIt_AndDoesNotFlipBack()
        {
            WheelNavigator nav = NewNavigator();
            Rest(nav, UpLeft, Delay + 0.05f);
            Assert.That(nav.InFolder, Is.True);
            Assert.That(nav.HoveringBack, Is.True, "Back sits where the folder slot was.");

            // Keep resting in the same spot: must not close again on its own.
            Rest(nav, UpLeft, Delay * 3f);
            Assert.That(nav.InFolder, Is.True);
            Assert.That(nav.Pick().IsNone, Is.True, "Releasing on Back picks nothing.");
        }

        [Test]
        public void InsideFolder_PicksOption()
        {
            WheelNavigator nav = NewNavigator();
            Rest(nav, UpLeft, Delay + 0.05f);
            // Folder ring has 5 wedges starting at the folder angle (315 deg): Back, then options clockwise.
            Vector2 firstOption = WheelMath.DirectionFromIndex(1, FolderOptions + 1, nav.RingFirstAngle);
            nav.Update(firstOption, Frame, Delay);
            WheelPick pick = nav.Pick();
            Assert.That(pick.Slot, Is.EqualTo(FolderSlot));
            Assert.That(pick.Option, Is.EqualTo(0));
        }

        [Test]
        public void ReturningToBack_ClosesFolderAfterRest()
        {
            WheelNavigator nav = NewNavigator();
            Rest(nav, UpLeft, Delay + 0.05f);
            Vector2 option = WheelMath.DirectionFromIndex(2, FolderOptions + 1, nav.RingFirstAngle);
            nav.Update(option, Frame, Delay);
            Rest(nav, UpLeft, Delay + 0.05f);
            Assert.That(nav.InFolder, Is.False);
            Assert.That(nav.Hovered, Is.EqualTo(FolderSlot));
        }

        [Test]
        public void StickLetGo_KeepsLastHover()
        {
            WheelNavigator nav = NewNavigator();
            nav.Update(Down, Frame, Delay);
            Rest(nav, null, 1f);
            Assert.That(nav.Pick().Slot, Is.EqualTo(4));
        }

        [Test]
        public void VariantRoll_CoversAllLooksAndStaysInRange()
        {
            var seen = new HashSet<int>();
            var choices = new List<int>();
            for (int i = 0; i < 60; i++)
                choices.Add(1);
            foreach (PlannedSpawn spawn in MessLayoutPlanner.PlanSeeded(choices, 11, 1f))
            {
                int variant = spawn.VariantIndex(3);
                Assert.That(variant, Is.InRange(0, 2));
                seen.Add(variant);
            }
            Assert.That(seen.Count, Is.EqualTo(3));
            Assert.That(new PlannedSpawn(0, 0, 0f, 0.99f).VariantIndex(1), Is.EqualTo(0));
        }
    }
}
