using System;
/// <summary>
/// Defines a scaling entry for a specific stat category with an associated multiplier.
/// </summary>
[Serializable]
public class StatScalingEntry {
    /// <summary>
    /// The category of the stat to which this scaling entry applies.
    /// </summary>
    public EStatCategory StatType;

    /// <summary>
    /// The multiplier used for scaling the stat.
    /// </summary>
    public float Multiplier;
}