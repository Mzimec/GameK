using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base interface for all stats.
/// </summary>
public interface IStatBase {
    /// <summary>
    /// Gets the type of this stat.
    /// </summary>
    public StatTypeSO StatType { get; }
}

/// <summary>
/// Represents a stat with a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the stat value.</typeparam>
public interface IStat<T> : IStatBase {
    /// <summary>
    /// Gets the current value of the stat.
    /// </summary>
    public T Value { get; }
}

/// <summary>
/// Represents a stat whose value can be modified by modifiers.
/// </summary>
/// <typeparam name="T">Type of the stat value.</typeparam>
public interface IModifiableStat<T> : IStat<T> {
    /// <summary>
    /// Adds a modifier to this stat.
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    public void AddModifier(IStatModifier<T> modifier);

    /// <summary>
    /// Removes a modifier from this stat.
    /// </summary>
    /// <param name="modifier">The modifier to remove.</param>
    public void RemoveModifier(IStatModifier<T> modifier);

    /// <summary>
    /// Removes all modifiers originating from a specific source.
    /// </summary>
    /// <param name="sourceId">The source identification which modifiers should be removed.</param>
    public void RemoveAllModifiers(string sourceId);

    /// <summary>
    /// Resets the stat value to its base value and clears all modifiers.
    /// </summary>
    public void ResetValue();
}

/// <summary>
/// Represents a resource stat that has both a maximum and a current value (like HP or Mana).
/// </summary>
/// <typeparam name="T">Type of the stat value.</typeparam>
public interface IResourceStat<T> : IModifiableStat<T> {
    /// <summary>
    /// Gets the current value of the resource.
    /// </summary>
    public T CurrentValue { get; }

    /// <summary>
    /// Modifies the current value of the resource.
    /// </summary>
    /// <param name="value">The amount to modify the current value by (can be positive or negative).</param>
    public void ModifyCurrentValue(T value);
}

/// <summary>
/// Represents a stat that can regenerate over time.
/// </summary>
/// <typeparam name="T">Type of the stat value.</typeparam>
/// <typeparam name="TStat">Type of the underlying stat.</typeparam>
public interface IRegeneratable<T, TStat>
    where TStat : IStat<T> {

    /// <summary>
    /// Gets the regeneration stat associated with this stat.
    /// </summary>
    public TStat Regeneration { get; }

    /// <summary>
    /// Applies regeneration to the stat.
    /// </summary>
    public void Regenerate();
}

/// <summary>
/// A modifiable stat implementation that supports adding and removing modifiers.
/// </summary>
/// <typeparam name="T">Type of the stat value.</typeparam>
public class ModifiableStat<T> : IModifiableStat<T> {
    private T _baseValue;
    protected virtual T _value { get; set; }
    private IStatModifiers<T> _modifiers;


    /// <summary>
    /// Creates a new modifiable stat with a base value and stat type.
    /// </summary>
    /// <param name="baseValue">The base value of the stat.</param>
    /// <param name="tStat">The stat type definition.</param>
    public ModifiableStat(T baseValue, StatTypeSO tStat = null) {
        _baseValue = baseValue;
        _modifiers = StatModifiersFactory.Create<T>();
        _value = CalculateValue();
        StatType = tStat;
    }

    /// <summary>
    /// Initializes a new modifiable stat with default values.
    /// </summary>
    public ModifiableStat() {
        _baseValue = default;
        _modifiers = StatModifiersFactory.Create<T>();
        _value = CalculateValue();
        StatType = null;
    }

    /// <inheritdoc/>
    public StatTypeSO StatType { get; }

    /// <summary>
    /// Calculates the current value of the stat by applying all modifiers to the base value.
    /// </summary>
    /// <returns></returns>
    private T CalculateValue() => _modifiers.ModifyValue(BaseValue);

    /// <summary>
    /// Gets or sets the base value of the stat. Setting this will recalculate the current value.
    /// </summary>
    public T BaseValue { 
        get => _baseValue;
        set {
            _baseValue = value;
            _value = CalculateValue();
        }
    }

    /// <inheritdoc/>
    public virtual T Value => _value;

    /// <inheritdoc/>
    public void AddModifier(IStatModifier<T> modifier) {
        _modifiers.Add(modifier);
        _value = CalculateValue();
    }

    /// <inheritdoc/>
    public void RemoveModifier(IStatModifier<T> modifier) {
        _modifiers.Remove(modifier);
        _value = CalculateValue();
    }

    /// <inheritdoc/>
    public void RemoveAllModifiers(string sourceId) {
        _modifiers.RemoveAll(sourceId);
        _value = CalculateValue();
    }

    /// <inheritdoc/>
    public void ResetValue() { 
        _modifiers.Clear();
        _value = CalculateValue();
    }
}

/// <summary>
/// A modifiable stat that is also comparable and optionally clamped between a minimum and maximum value.
/// </summary>
/// <typeparam name="T">Type of the stat value, must implement IComparable<T></T>;.</typeparam>
public class ComparableModifiableStat<T> : ModifiableStat<T>, IComparable<ComparableModifiableStat<T>>
    where T : IComparable<T> {

    private readonly T _minValue;
    private readonly T _maxValue;

    /// <summary>
    /// Creates a new comparable modifiable stat with specified base value, stat type, minimum, and maximum values.
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="tStat"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public ComparableModifiableStat(T baseValue, T minValue, T maxValue, StatTypeSO tStat = null) : base(baseValue,tStat) {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    /// <summary>
    /// Initializes a new comparable modifiable stat with default values.
    /// </summary>
    public ComparableModifiableStat() : base() {
        _minValue = default;
        _maxValue = default;
    }

    /// <inheritdoc/>
    public int CompareTo(ComparableModifiableStat<T> other) {
        if (other == null) return 1;
        return Value.CompareTo(other.Value);
    }

    /// <inheritdoc/>
    public override T Value {
        get {
            var val = base.Value;
            if (val.CompareTo(_minValue) < 0) return _minValue;
            if (val.CompareTo(_maxValue) > 0) return _maxValue;
            return val;
        }
    }
}

/// <summary>
/// Abstract base class for resource stats (with a current value).
/// </summary>
/// <typeparam name="T">Type of the resource stat.</typeparam>
public abstract class AbstractResourceStat<T> : ModifiableStat<T>, IResourceStat<T> {
    protected T _currentValue;

    /// <summary>
    /// Gets or sets the maximum value of the resource stat. Setting this will adjust the current value if necessary.
    /// </summary>
    protected override T _value { get => base._value; set {
            base._value = value;
            _currentValue = Value;
        }
    }

    /// <summary>
    /// Creates a new resource stat with a base value and stat type.
    /// </summary>
    /// <param name="baseValue">Base value of the stat</param>
    /// <param name="tStat">Stat type of the stat</param>
    public AbstractResourceStat(T baseValue, StatTypeSO tStat = null) : base(baseValue, tStat) {
        _currentValue = baseValue;
    }

    /// <summary>
    /// Initializes a new resource stat with default values.
    /// </summary>
    public AbstractResourceStat() : base() {
        _currentValue = default;
    }

    /// <inheritdoc/>
    public T CurrentValue => _currentValue;

    /// <summary>
    /// Modifies the current value of the resource.
    /// </summary>
    /// <param name="value">Value by which the current value of the stat changes</param>
    public abstract void ModifyCurrentValue(T value);
}

/// <summary>
/// Integer-based resource stat with clamped current value (e.g., HP or Mana).
/// </summary>
public class IntResourceStat : AbstractResourceStat<int> {
    protected override int _value {
        get => base._value; set {
            base._value = value;
            _currentValue = Mathf.Clamp(_currentValue, 0, Value);
        }
    }

    /// <inheritdoc/>
    public IntResourceStat(int baseValue, StatTypeSO tStat = null) : base(baseValue, tStat) { }
    /// <inheritdoc/>
    public IntResourceStat() : base() { }

    /// <inheritdoc/>
    public override void ModifyCurrentValue(int value) => _currentValue = Mathf.Clamp(_currentValue + value, 0, Value);
}

/// <summary>
/// Integer-based comparable modifiable stat.
/// </summary>
public class IntComparableStat : ComparableModifiableStat<int> {
    /// <inheritdoc/>
    public IntComparableStat(int baseValue, int minValue, int maxValue, StatTypeSO tStat = null) : base(baseValue, minValue, maxValue, tStat) { }
    /// <inheritdoc/>
    public IntComparableStat() : base() { }

}


