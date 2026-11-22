using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Tuning
{
    public enum TuningFieldKind { Float, Int, Bool }

    /// <summary>
    /// One tweakable value on a TuningConfig, found by reflection. Values pass
    /// through as floats so every kind can drive a slider or toggle.
    /// </summary>
    public sealed class TuningField
    {
        // Used when a field has no [Range]: slider goes from 0 to value x multiplier.
        const float FallbackRangeMultiplier = 4f;
        const float FallbackMinimumMax = 1f;

        readonly FieldInfo field;

        public TuningConfig Config { get; }
        public TuningFieldKind Kind { get; }
        public string Label { get; }
        public string Tooltip { get; }
        public string Header { get; }
        public float Min { get; }
        public float Max { get; }

        TuningField(TuningConfig config, FieldInfo field, TuningFieldKind kind)
        {
            Config = config;
            this.field = field;
            Kind = kind;
            Label = Nicify(field.Name);
            Tooltip = field.GetCustomAttribute<TooltipAttribute>()?.tooltip;
            Header = field.GetCustomAttribute<HeaderAttribute>()?.header;

            var range = field.GetCustomAttribute<RangeAttribute>();
            if (kind == TuningFieldKind.Bool)
            {
                Min = 0f;
                Max = 1f;
            }
            else if (range != null)
            {
                Min = range.min;
                Max = range.max;
            }
            else
            {
                Min = 0f;
                Max = Mathf.Max(FallbackMinimumMax, Mathf.Abs(GetValue()) * FallbackRangeMultiplier);
            }
        }

        public float GetValue()
        {
            object value = field.GetValue(Config);
            switch (Kind)
            {
                case TuningFieldKind.Bool: return (bool)value ? 1f : 0f;
                case TuningFieldKind.Int: return (int)value;
                default: return (float)value;
            }
        }

        public void SetValue(float value)
        {
            switch (Kind)
            {
                case TuningFieldKind.Bool: field.SetValue(Config, value > 0.5f); break;
                case TuningFieldKind.Int: field.SetValue(Config, Mathf.RoundToInt(value)); break;
                default: field.SetValue(Config, value); break;
            }
            Config.NotifyChanged();
        }

        /// <summary>All public float, int and bool fields of a config, in declaration order.</summary>
        public static List<TuningField> Collect(TuningConfig config)
        {
            var result = new List<TuningField>();
            FieldInfo[] fields = config.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in fields)
            {
                if (info.IsDefined(typeof(NonSerializedAttribute)) || info.IsDefined(typeof(HideInInspector)))
                    continue;
                if (TryGetKind(info.FieldType, out TuningFieldKind kind))
                    result.Add(new TuningField(config, info, kind));
            }
            return result;
        }

        static bool TryGetKind(Type type, out TuningFieldKind kind)
        {
            if (type == typeof(float)) { kind = TuningFieldKind.Float; return true; }
            if (type == typeof(int)) { kind = TuningFieldKind.Int; return true; }
            if (type == typeof(bool)) { kind = TuningFieldKind.Bool; return true; }
            kind = default;
            return false;
        }

        /// <summary>"walkSpeed" becomes "Walk Speed".</summary>
        public static string Nicify(string name)
        {
            var builder = new StringBuilder(name.Length + 4);
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (i == 0)
                    builder.Append(char.ToUpperInvariant(c));
                else
                {
                    if (char.IsUpper(c) && !char.IsUpper(name[i - 1]))
                        builder.Append(' ');
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
    }
}
