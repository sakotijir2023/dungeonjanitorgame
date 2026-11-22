using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>Pure geometry for radial menus. Angles are degrees clockwise from straight up.</summary>
    public static class WheelMath
    {
        const float FullTurn = 360f;

        /// <summary>Clockwise angle from up, 0..360.</summary>
        public static float AngleOf(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            return Mathf.Repeat(angle, FullTurn);
        }

        /// <summary>Which of <paramref name="count"/> equal wedges a direction points into. Wedge 0 is centred on firstAngle.</summary>
        public static int IndexFromDirection(Vector2 direction, int count, float firstAngle)
        {
            if (count <= 0)
                return -1;
            float wedge = FullTurn / count;
            float relative = Mathf.Repeat(AngleOf(direction) - firstAngle + wedge * 0.5f, FullTurn);
            return Mathf.Min(count - 1, Mathf.FloorToInt(relative / wedge));
        }

        /// <summary>
        /// Like IndexFromDirection, but keeps <paramref name="current"/> until the
        /// direction is more than <paramref name="stickyDegrees"/> past its border.
        /// Stops the pick flickering between neighbours when aiming near a border.
        /// </summary>
        public static int StickyIndex(Vector2 direction, int count, float firstAngle, int current, float stickyDegrees)
        {
            int raw = IndexFromDirection(direction, count, firstAngle);
            if (current < 0 || current >= count || raw == current || stickyDegrees <= 0f)
                return raw;
            float wedge = FullTurn / count;
            float centre = firstAngle + current * wedge;
            float offset = Mathf.Abs(Mathf.DeltaAngle(AngleOf(direction), centre));
            return offset <= wedge * 0.5f + stickyDegrees ? current : raw;
        }

        /// <summary>Unit direction to the centre of wedge <paramref name="index"/> (y up).</summary>
        public static Vector2 DirectionFromIndex(int index, int count, float firstAngle)
        {
            float angle = (firstAngle + index * FullTurn / count) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }

        /// <summary>Angle of slot <paramref name="index"/> on a ring of <paramref name="count"/> starting at the top.</summary>
        public static float SlotAngle(int index, int count) => count > 0 ? index * FullTurn / count : 0f;
    }
}
