using System.Collections.Generic;

namespace DungeonJanitor.Messes
{
    /// <summary>One planned spawn: which point, which of its allowed families, and a 0..1 roll for hardness.</summary>
    public readonly struct PlannedSpawn
    {
        public readonly int PointIndex;
        public readonly int ChoiceIndex;
        public readonly float HardnessRoll;

        public PlannedSpawn(int pointIndex, int choiceIndex, float hardnessRoll)
        {
            PointIndex = pointIndex;
            ChoiceIndex = choiceIndex;
            HardnessRoll = hardnessRoll;
        }
    }

    /// <summary>
    /// Decides which spawn points get a mess in a seeded run. Pure and
    /// deterministic: the same seed always gives the same room.
    /// </summary>
    public static class MessLayoutPlanner
    {
        /// <param name="choicesPerPoint">How many allowed families each point has (0 = never spawns).</param>
        /// <param name="fillChance">Chance (0..1) that each point gets a mess.</param>
        public static List<PlannedSpawn> PlanSeeded(IReadOnlyList<int> choicesPerPoint, int seed, float fillChance)
        {
            var random = new System.Random(seed);
            var plan = new List<PlannedSpawn>();
            for (int i = 0; i < choicesPerPoint.Count; i++)
            {
                // Always draw the same number of values per point so one point's
                // settings never shift the rolls of the points after it.
                double fillRoll = random.NextDouble();
                double choiceRoll = random.NextDouble();
                double hardnessRoll = random.NextDouble();

                int choices = choicesPerPoint[i];
                if (choices <= 0 || fillRoll >= fillChance)
                    continue;

                int choice = (int)(choiceRoll * choices);
                if (choice >= choices)
                    choice = choices - 1;
                plan.Add(new PlannedSpawn(i, choice, (float)hardnessRoll));
            }
            return plan;
        }
    }
}
