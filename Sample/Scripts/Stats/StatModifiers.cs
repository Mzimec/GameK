using System.Collections.Generic;
using System;
using System.Numerics;
using Unity.VisualScripting;
using System.Collections.Specialized;
using System.Linq;

/// <summary>
/// Base interface for a stat modifier.
/// </summary>
/// <typeparam name="T">The type of the stat value.</typeparam>
public interface IStatModifier<T> {
    /// <summary>
    /// Identifier of the source of this modifier.
    /// </summary>
    public string SourceId { get; }
    /// <summary>
    /// Modifies the given source value according to this modifier's logic.
    /// </summary>
    /// <param name="sourceValue">The original value to be modified.</param>
    /// <returns>The modified value.</returns>
    public T ModifyValue(T sourceValue);
}

/// <summary>
/// Represents a setter-type stat modifier.
/// </summary>
public interface ISetterModifier<T> : IStatModifier<T> { }

/// <summary>
/// Represents a flat-additive or negative stat modifier.
/// </summary>
public interface IFlatModifier<T> : IStatModifier<T> { }

/// <summary>
/// Represents a percentage-based stat modifier.
/// </summary>
public interface IPercentageModifier<T> : IStatModifier<T> { }

/// <summary>
/// Represents a stat modifier that clamps the value.
/// </summary>
public interface IClampModifier<T> : IStatModifier<T> {
    /// <summary>
    /// True if this is a buff clamp (increase minimum), false if debuff (reduce maximum).
    /// </summary>
    public bool IsBuff { get; }
}

/// <summary>
/// Base implementation for stat modifiers.
/// </summary>
/// <Typeparam name="T">The type of the stat value.</typeparam>
public abstract class StatModifierBase<T> : IStatModifier<T> {
    private string _sourceId;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModifierBase{T}"/> class.
    /// </summary>
    /// <param name="sourceId">The identificaton value of the source</param>
    public StatModifierBase(string sourceId) {
        _sourceId = sourceId;
    }

    /// <inheritdoc/>
    public string SourceId => _sourceId;

    /// <inheritdoc/>
    public abstract T ModifyValue(T sourceValue);
}

/// <summary>
/// An implementation of stat modifier that sets the value to a specific value.
/// </summary>
/// <typeparam name="T">The type of the stat value</typeparam>
public class SetterStatModifier<T> : StatModifierBase<T>, ISetterModifier<T> {
    private T _modificationValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetterStatModifier{T}"/> class.
    /// </summary>
    /// <param name="modificationValue">Value on which the modifier sets the stat</param>
    /// <param name="sourceId">The identification value of the source</param>
    public SetterStatModifier(T modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }

    /// <inheritdoc/>
    public override T ModifyValue(T sourceValue) => _modificationValue;
}

// T should be a float or int type. In newer Unity where INumber<T> is available, we can use that instead of dynamic.
// dynamic is not well functioning with Unity. Option for universal class is to make TypeOf fuction.
/*public class FlatStatModifier<T> : StatModifierBase<T>, IFlatModifier<T> {
    private T _modificationValue;
    public FlatStatModifier(T modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
    public override T ModifyValue(T sourceValue) {
        try {
            return (T)((dynamic)sourceValue + (dynamic)_modificationValue);
        }
        catch (InvalidCastException e) {
            throw new InvalidOperationException($"Cannot add {typeof(T)} to {typeof(T)}", e);
        }
    }

}*/

/// <summary>
/// An implementation of stat modifier of float value that adds or removes a flat value to or from the stat.
/// </summary>
public class FloatFlatStatModifier : StatModifierBase<float>, IFlatModifier<float> {
    private float _modificationValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatFlatStatModifier"/> class.
    /// </summary>
    /// <param name="modificationValue">Value by which it chages teh stat</param>
    /// <param name="sourceId">Teh identification value of the source</param>
    public FloatFlatStatModifier(float modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }

    /// <inheritdoc/>
    public override float ModifyValue(float sourceValue) => sourceValue + _modificationValue;
}


/// <summary>
/// An implementation of stat modifier of int value that adds or removes a flat value to or from the stat.
/// </summary>
public class IntFlatStatModifier : StatModifierBase<int>, IFlatModifier<int> {
    private int _modificationValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntFlatStatModifier"/> class.
    /// </summary>
    /// <param name="modificationValue">Value by which it chages the stat</param>
    /// <param name="sourceId">The identification value of the source</param>
    public IntFlatStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }

    /// <inheritdoc/>
    public override int ModifyValue(int sourceValue) => sourceValue + _modificationValue;
}

// T should be a numeric type. In newer Unity where INumber<T> is available, we can use that instead of dynamic.
// dynamic is not well functioning with Unity. Option for universal class is to make TypeOf fuction.
/*public class PercentageStatModifier<T> : StatModifierBase<T>, IPercentageModifier<T> {
    private int _modificationValue;
    public PercentageStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
    public override T ModifyValue(T sourceValue) {
        try {
            return (T)((dynamic)sourceValue * (1f + _modificationValue / 100f));
        }
        catch (InvalidCastException e) {
            throw new InvalidOperationException($"Cannot multiply {typeof(T)} by {typeof(T)}", e);
        }
    }
}*/

/// <summary>
/// An implementation of stat modifier of float value that modifies the stat by a percentage.
/// </summary>
public class FloatPercentageStatModifier : StatModifierBase<float>, IPercentageModifier<float> {
    private int _modificationValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatPercentageStatModifier"/> class.
    /// </summary>
    /// <param name="modificationValue">The value by which it multiples the stat</param>
    /// <param name="sourceId">The identification value of the source</param>
    public FloatPercentageStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }

    /// <inheritdoc/>
    public override float ModifyValue(float sourceValue) => sourceValue * (1f + _modificationValue / 100f);
}

/// <summary>
/// An implementation of stat modifier of int value that modifies the stat by a percentage.
/// </summary>
public class IntPercentageStatModifier : StatModifierBase<int>, IPercentageModifier<int> {
    private int _modificationValue;
    /// <summary>
    /// Initializes a new instance of the <see cref="IntPercentageStatModifier"/> class.
    /// </summary> 
    /// <param name="modificationValue">The value by which it multiples the stat</param>
    /// <param name="sourceId">The identification value of the source</param>
    public IntPercentageStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }

    /// <inheritdoc/>
    public override int ModifyValue(int sourceValue) => (int)(sourceValue * (1f + _modificationValue / 100f));
}

// T should be a float or int type. In newer Unity where INumber<T> is available, we can use that instead of dynamic.
// dynamic is not well functioning with Unity. Option for universal class is to make TypeOf fuction.
/*public class ClampStatModifier<T> : StatModifierBase<T>, IClampModifier<T> {
    private T _modificationValue;
    public bool IsBuff { get; }
    public ClampStatModifier(T modificationValue, bool isBuff, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
        this.IsBuff = isBuff;
    }
    public override T ModifyValue(T sourceValue) {
        try {
            return (T)((dynamic)sourceValue > (dynamic)_modificationValue ? _modificationValue : sourceValue);
        }
        catch (InvalidCastException e) {
            throw new InvalidOperationException($"Cannot compare {typeof(T)} to {typeof(T)}", e);
        }
    }
}*/


/// <summary>
/// An implementation of stat modifier of float value that clamps the stat to a minimum or maximum value.
/// </summary>
public class FloatClampStatModifier : StatModifierBase<float>, IClampModifier<float> {
    private float _value;

    /// <inheritdoc/>
    public bool IsBuff { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatClampStatModifier"/> class.
    /// </summary>
    /// <param name="value">Value on which it clamps the stat</param>
    /// <param name="isBuff">Flag to set the IsBuff</param>
    /// <param name="sourceId">The identification value of the source</param>
    public FloatClampStatModifier(float value, bool isBuff, string sourceId) : base(sourceId) {
        this._value = value;
        this.IsBuff = isBuff;
    }

    /// <inheritdoc/>
    public override float ModifyValue(float sourceValue) => IsBuff ? Math.Max(sourceValue, _value) : Math.Min(sourceValue, _value);
}

/// <summary>
/// An implementation of stat modifier of int value that clamps the stat to a minimum or maximum value.
/// </summary>
public class IntClampStatModifier : StatModifierBase<int>, IClampModifier<int> {
    private int _value;

    /// <inheritdoc/>
    public bool IsBuff { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntClampStatModifier"/> class.
    /// </summary>
    /// <param name="value">Value on which it clamps the stat</param>
    /// <param name="isBuff">Flag to set the IsBuff</param>
    /// <param name="sourceId">The identification value of the source</param>
    public IntClampStatModifier(int value, bool isBuff, string sourceId) : base(sourceId) {
        this._value = value;
        this.IsBuff = isBuff;
    }

    /// <inheritdoc/>
    public override int ModifyValue(int sourceValue) => IsBuff ? Math.Max(sourceValue, _value) : Math.Min(sourceValue, _value);
}


/// <summary>
/// Manages a collection of stat modifiers and applies them to a base value.
/// </summary>
/// <typeparam name="T">Type of the value of the Stat</typeparam>
public interface IStatModifiers<T> {
    /// <summary>
    /// Modifies the given source value by applying all registered modifiers in the correct order.
    /// </summary>
    /// <param name="sourceValue">Original value of the stat</param>
    /// <returns>Return final modified value</returns>
    public T ModifyValue(T sourceValue);

    /// <summary>
    /// Adds a new modifier to the collection.
    /// </summary>
    /// <param name="modifier">Modifier to be added</param>
    public void Add(IStatModifier<T> modifier);

    /// <summary>
    /// Removes a modifier from the collection.
    /// </summary>
    /// <param name="modifier">Modifier to be removed</param>
    public void Remove(IStatModifier<T> modifier);

    /// <summary>
    /// Removes all modifiers originating from a specific source.
    /// </summary>
    /// <param name="sourceId">The source identification of which modifiers should be removed</param>
    public void RemoveAll(string sourceId);

    /// <summary>
    /// Clears all modifiers from the collection.
    /// </summary>
    public void Clear();
}

/// <summary>
/// A basic implementation of <see cref="IStatModifiers{T}"/> that only supports setter modifiers.
/// </summary>
/// <typeparam name="T">Type of the value of the stat</typeparam>
public class StatModifiers<T> : IStatModifiers<T> {
    private HashSet<ISetterModifier<T>> _modifiers = new();

    /// <inheritdoc/>
    public T ModifyValue(T sourceValue) {
        foreach (var setter in _modifiers) {
            sourceValue = setter.ModifyValue(sourceValue);
        }
        return sourceValue;
    }

    /// <inheritdoc/>
    public void Add(IStatModifier<T> modifier) {
        if (modifier is SetterStatModifier<T> setter) {
            _modifiers.Add(setter);
        }
        else {
            throw new ArgumentException($"Modifier of type {modifier.GetType()} is not supported.");
        }
    }

    /// <inheritdoc/>
    public void Remove(IStatModifier<T> modifier) {
        if (modifier is SetterStatModifier<T> setter) {
            _modifiers.Remove(setter);
        }
    }

    /// <inheritdoc/>
    public void RemoveAll(string sourceId) => _modifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);

    /// <inheritdoc/>
    public void Clear() => _modifiers.Clear();
}

/// <summary>
/// An Abstract implementation of <see cref="IStatModifiers{T}"/> for numeric types that supports flat, percentage and clamp modifiers.
/// </summary>
/// <typeparam name="T">Type of the value of the stat</typeparam>
/// <typeparam name="TFlat">Type of the flat modifiers</typeparam>
/// <typeparam name="TPercent">Type of the percentage modifiers</typeparam>
/// <typeparam name="TClamp">Type of the clamp modifiers</typeparam>
public abstract class NumericsStatModifiers<T,TFlat, TPercent, TClamp> : IStatModifiers<T> 
    where TFlat : IFlatModifier<T> 
    where TPercent : IPercentageModifier<T>
    where TClamp : IClampModifier<T> {

    private HashSet<IStatModifier<T>> _setters = new();
    private HashSet<IStatModifier<T>> _flatModifiers = new();
    private HashSet<IStatModifier<T>> _percentageModifiers = new();
    private HashSet<IStatModifier<T>> _positiveClampers = new();
    private HashSet<IStatModifier<T>> _negativeClampers = new();

    ///<inheritdoc/>
    public T ModifyValue(T sourceValue) {
        sourceValue = _setters.Count > 0 ? _setters.Max(modifier => modifier.ModifyValue(sourceValue)) : sourceValue;
        foreach (var fmod in _flatModifiers) sourceValue = fmod.ModifyValue(sourceValue);
        foreach (var pmod in _percentageModifiers) sourceValue = pmod.ModifyValue(sourceValue);
        foreach (var cpmod in _positiveClampers) sourceValue = cpmod.ModifyValue(sourceValue);
        foreach (var cnmod in _negativeClampers) sourceValue = cnmod.ModifyValue(sourceValue);
        return sourceValue;
    }

    ///<inheritdoc/>
    public void Add(IStatModifier<T> modifier) {
        switch (modifier) {
            case SetterStatModifier<T> setter:
                _setters.Add(setter);
                break;
            case TFlat flat:
                _flatModifiers.Add(flat);
                break;
            case TPercent percentage:
                _percentageModifiers.Add(percentage);
                break;
            case TClamp clamped:
                if (clamped.IsBuff) {
                    _positiveClampers.Add(clamped);
                }
                else {
                    _negativeClampers.Add(clamped);
                }
                break;
            default:
                throw new ArgumentException($"Modifier of type {modifier.GetType()} is not supported.");
        }
    }

    ///<inheritdoc/>
    public void Remove(IStatModifier<T> modifier) {
        switch (modifier) {
            case SetterStatModifier<T> smod:
                _setters.Remove(smod);
                break;
            case TFlat fmod:
                _flatModifiers.Remove(fmod);
                break;
            case TPercent pmod:
                _percentageModifiers.Remove(pmod);
                break;
            case TClamp cmod:
                if (cmod.IsBuff) {
                    _positiveClampers.Remove(cmod);
                }
                else {
                    _negativeClampers.Remove(cmod);
                }
                break;
            default:
                throw new ArgumentException($"Modifier of type {modifier.GetType()} is not supported.");
        }
    }

    ///<inheritdoc/>
    public void RemoveAll(string sourceId) {
        _setters.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _flatModifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _percentageModifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _positiveClampers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _negativeClampers.RemoveWhere(modifier => modifier.SourceId == sourceId);
    }

    ///<inheritdoc/>
    public void Clear() {
        _setters.Clear();
        _flatModifiers.Clear();
        _percentageModifiers.Clear();
        _positiveClampers.Clear();
        _negativeClampers.Clear();
    }
}

/// <summary>
/// An implementation of <see cref="NumericsStatModifiers{T,TFlat,TPercent,TClamp}"/> for int type that supports flat, percentage and clamp modifiers.
/// </summary>
public class IntStatModifiers : NumericsStatModifiers<int, IntFlatStatModifier, IntPercentageStatModifier, IntClampStatModifier> {}

/// <summary>
/// An implementation of <see cref="NumericsStatModifiers{T,TFlat,TPercent,TClamp}"/> for float type that supports flat, percentage and clamp modifiers.
/// </summary>
public class FloatStatModifiers : NumericsStatModifiers<float, FloatFlatStatModifier, FloatPercentageStatModifier, FloatClampStatModifier> { }


/// <summary>
/// A factory class for creating appropriate <see cref="IStatModifiers{T}"/> instances based on the type parameter.
///</summary>
public static class StatModifiersFactory {
    /// <summary>
    /// Creates an instance of <see cref="IStatModifiers{T}"/> based on the type parameter T.
    /// </summary>
    /// <returns>An instance of <see cref="IStatModifiers{T}"/>.</returns>
    public static IStatModifiers<T> Create<T>() {
        if (typeof(T) == typeof(int)) {
            return new IntStatModifiers() as IStatModifiers<T>;
        }
        else if (typeof(T) == typeof(float)) {
            return new FloatStatModifiers() as IStatModifiers<T>;
        }
        else {
            return new StatModifiers<T>();
        }
    }
}