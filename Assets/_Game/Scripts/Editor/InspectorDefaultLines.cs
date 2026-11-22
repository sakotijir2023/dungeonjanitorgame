using System.Collections.Generic;
using DungeonJanitor.Inspection;
using DungeonJanitor.Messes;
using DungeonJanitor.Scoring;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// The Inspector's starter lines. Only written into a brand-new (empty)
    /// InspectorLines asset, so the owner's edits in Unity are never overwritten.
    /// Voice: dry, sarcastic, mean, never swears. Even good grades get a jab;
    /// a perfect score is met with disbelief.
    /// </summary>
    public static class InspectorDefaultLines
    {
        public static void Fill(InspectorLines lines)
        {
            lines.byGrade = new List<InspectorLines.GradeLines>
            {
                Grade(DungeonJanitor.Scoring.Grade.S,
                    "...Hold on. I'm checking under the rug. ...I'm checking again.",
                    "Spotless. Who did you bribe? I want a name.",
                    "Thirty years I've done this job. This has never happened. I'm filing a complaint about how suspicious it is.",
                    "Perfect. Wonderful. Now my pen has nothing to do. I hope you're happy.",
                    "No. No, there's a speck right— no. Fine. FINE. It's perfect. Get out."),
                Grade(DungeonJanitor.Scoring.Grade.A,
                    "Almost competent. Don't let it go to your head; there isn't much room in there.",
                    "An A. I'm as surprised as you are. Actually, I'm more surprised.",
                    "Acceptable. The heroes might even live long enough to notice.",
                    "Good work. I'm told I'm supposed to say that occasionally.",
                    "Nearly clean. 'Nearly' is doing a lot of heavy lifting there."),
                Grade(DungeonJanitor.Scoring.Grade.B,
                    "Mediocre, but enthusiastically so.",
                    "It's fine. 'Fine' is also what I call lukewarm soup.",
                    "A solid B. Like your personality: forgettable, but not technically a hazard.",
                    "Some of this is clean. I can tell because it stands out.",
                    "The heroes will only mostly slip and die. Progress, I suppose."),
                Grade(DungeonJanitor.Scoring.Grade.C,
                    "Average. Deeply, profoundly average. It's almost a talent.",
                    "Did you clean this room, or just visit it?",
                    "I've seen goblins tidy up better. Goblins eat the furniture.",
                    "C. As in 'Could you not?'",
                    "Half of this is clean. The other half is a cry for help."),
                Grade(DungeonJanitor.Scoring.Grade.D,
                    "I'd call this cleaning, but I don't lie on official forms.",
                    "The mop was right there. Did you two have a falling out?",
                    "This room is a liability. So are you.",
                    "I'm docking you for the smell alone. Was that you, or the room?",
                    "D. For the grade, and for how many messes you seem to have noticed."),
                Grade(DungeonJanitor.Scoring.Grade.F,
                    "Did you clean anything, or just carry the vacuum around for emotional support?",
                    "I've graded crime scenes cleaner than this. Some were still in progress.",
                    "F. I'd go lower, but the form won't let me.",
                    "This dungeon is dirtier than when you arrived. That shouldn't even be possible.",
                    "Go home. Think about what you've done. Then don't come back and do it again."),
            };

            lines.nothingCleaned = new[]
            {
                "You were paid to clean. You cleaned nothing. I almost admire the commitment.",
                "Not one mess. Not one. Were you on a break the entire time, or just a waste of a uniform?",
            };

            lines.tooToughHints = new List<InspectorLines.FamilyHints>
            {
                Hints(MessFamily.Stain,
                    "I see you didn't even try to clean that stain, hmm? Maybe stop being a degenerate and invest in your tools.",
                    "That stain laughed at your little mop. I heard it. Buy a better one."),
                Hints(MessFamily.Goo,
                    "That dark goo is still glued to the floor. Your pathetic vacuum won't budge it. Upgrade it, or keep embarrassing us both.",
                    "The black goo stays, I see. A real janitor would own a real vacuum."),
            };

            lines.genericTooToughHints = new[]
            {
                "Some of this mess was beyond your sad little toolkit. Spend money on yourself. On tools, I mean. Not snacks.",
            };
        }

        static InspectorLines.GradeLines Grade(Grade grade, params string[] remarks) =>
            new InspectorLines.GradeLines { grade = grade, lines = remarks };

        static InspectorLines.FamilyHints Hints(MessFamily family, params string[] hints) =>
            new InspectorLines.FamilyHints { family = family, lines = hints };
    }
}
