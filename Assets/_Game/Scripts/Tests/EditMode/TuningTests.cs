using System.IO;
using System.Linq;
using DungeonJanitor.Player;
using DungeonJanitor.Tuning;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class TuningTests
    {
        const string ConfigName = "MovementConfig";

        MovementConfig config;

        [SetUp]
        public void SetUp()
        {
            config = ScriptableObject.CreateInstance<MovementConfig>();
            config.name = ConfigName;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(config);

        [Test]
        public void Collect_FindsFloatAndBoolFieldsWithRanges()
        {
            var fields = TuningField.Collect(config);
            TuningField walk = fields.Single(f => f.Label == "Walk Speed");
            Assert.That(walk.Kind, Is.EqualTo(TuningFieldKind.Float));
            Assert.That(walk.Min, Is.LessThan(walk.Max));
            Assert.That(walk.Header, Is.EqualTo("Ground movement"));
            Assert.That(fields.Single(f => f.Label == "Invert Y").Kind, Is.EqualTo(TuningFieldKind.Bool));
        }

        [Test]
        public void SetValue_WritesThroughAndRaisesChanged()
        {
            bool changed = false;
            config.Changed += () => changed = true;
            TuningField jump = TuningField.Collect(config).Single(f => f.Label == "Jump Height");

            jump.SetValue(2.5f);

            Assert.That(config.jumpHeight, Is.EqualTo(2.5f));
            Assert.That(changed, Is.True);
        }

        [Test]
        public void Preset_RoundTripsValues()
        {
            config.walkSpeed = 3.3f;
            config.invertY = true;
            string preset = PresetSerializer.Serialize(new[] { config });

            config.walkSpeed = 9f;
            config.invertY = false;
            int applied = PresetSerializer.Apply(preset, new[] { config });

            Assert.That(applied, Is.EqualTo(1));
            Assert.That(config.walkSpeed, Is.EqualTo(3.3f));
            Assert.That(config.invertY, Is.True);
        }

        [Test]
        public void Preset_IgnoresUnknownConfigs()
        {
            string preset = PresetSerializer.Serialize(new[] { config });
            var other = ScriptableObject.CreateInstance<MovementConfig>();
            other.name = "SomethingElse";
            Assert.That(PresetSerializer.Apply(preset, new[] { other }), Is.EqualTo(0));
            Object.DestroyImmediate(other);
        }

        [Test]
        public void Store_SavesListsLoadsAndDeletes()
        {
            string dir = Path.Combine(Path.GetTempPath(), "DJ_PresetTest_" + System.Guid.NewGuid().ToString("N"));
            var store = new PresetStore(dir);
            try
            {
                string saved = store.Save("  Floaty/Jump  ", "{}");
                Assert.That(saved, Is.EqualTo("Floaty_Jump"));
                Assert.That(store.ListNames(), Is.EquivalentTo(new[] { "Floaty_Jump" }));
                Assert.That(store.TryLoad(saved, out string json), Is.True);
                Assert.That(json, Is.EqualTo("{}"));
                store.Delete(saved);
                Assert.That(store.ListNames(), Is.Empty);
            }
            finally
            {
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            }
        }

        [Test]
        public void Nicify_SplitsCamelCase()
        {
            Assert.That(TuningField.Nicify("fallGravityMultiplier"), Is.EqualTo("Fall Gravity Multiplier"));
        }
    }
}
