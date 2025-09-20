using System.Collections.Generic;
using System;
using System.Numerics;
using Unity.VisualScripting;
using System.Collections.Specialized;
using System.Linq;

public interface IStatModifier<T> {
    public string SourceId { get; }
    public T ModifyValue(T sourceValue);
}
public interface ISetterModifier<T> : IStatModifier<T> { }

public interface IFlatModifier<T> : IStatModifier<T> { }

public interface IPercentageModifier<T> : IStatModifier<T> { }

public interface IClampModifier<T> : IStatModifier<T> {
    public bool IsBuff { get; }
}

public abstract class StatModifierBase<T> : IStatModifier<T> {
    private string _sourceId;
    public StatModifierBase(string sourceId) {
        _sourceId = sourceId;
    }
    public string SourceId => _sourceId;
    public abstract T ModifyValue(T sourceValue);
}

public class SetterStatModifier<T> : StatModifierBase<T>, ISetterModifier<T> {
    private T _modificationValue;
    public SetterStatModifier(T modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
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

public class FloatFlatStatModifier : StatModifierBase<float>, IFlatModifier<float> {
    private float _modificationValue;
    public FloatFlatStatModifier(float modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
    public override float ModifyValue(float sourceValue) => sourceValue + _modificationValue;
}

public class IntFlatStatModifier : StatModifierBase<int>, IFlatModifier<int> {
    private int _modificationValue;
    public IntFlatStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
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

public class FloatPercentageStatModifier : StatModifierBase<float>, IPercentageModifier<float> {
    private int _modificationValue;
    public FloatPercentageStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
    public override float ModifyValue(float sourceValue) => sourceValue * (1f + _modificationValue / 100f);
}

public class IntPercentageStatModifier : StatModifierBase<int>, IPercentageModifier<int> {
    private int _modificationValue;
    public IntPercentageStatModifier(int modificationValue, string sourceId) : base(sourceId) {
        this._modificationValue = modificationValue;
    }
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

public class FloatClampStatModifier : StatModifierBase<float>, IClampModifier<float> {
    private float _value;
    public bool IsBuff { get; }
    public FloatClampStatModifier(float value, bool isBuff, string sourceId) : base(sourceId) {
        this._value = value;
        this.IsBuff = isBuff;
    }
    public override float ModifyValue(float sourceValue) => IsBuff ? Math.Max(sourceValue, _value) : Math.Min(sourceValue, _value);
}

public class IntClampStatModifier : StatModifierBase<int>, IClampModifier<int> {
    private int _value;
    public bool IsBuff { get; }
    public IntClampStatModifier(int value, bool isBuff, string sourceId) : base(sourceId) {
        this._value = value;
        this.IsBuff = isBuff;
    }
    public override int ModifyValue(int sourceValue) => IsBuff ? Math.Max(sourceValue, _value) : Math.Min(sourceValue, _value);
}


public interface IStatModifiers<T> {
    public T ModifyValue(T sourceValue);
    public void Add(IStatModifier<T> modifier);
    public void Remove(IStatModifier<T> modifier);
    public void RemoveAll(string sourceId);
    public void Clear();
}

public class StatModifiers<T> : IStatModifiers<T> {
    private HashSet<ISetterModifier<T>> _modifiers = new();

    public T ModifyValue(T sourceValue) {
        foreach (var setter in _modifiers) {
            sourceValue = setter.ModifyValue(sourceValue);
        }
        return sourceValue;
    }

    public void Add(IStatModifier<T> modifier) {
        if (modifier is SetterStatModifier<T> setter) {
            _modifiers.Add(setter);
        }
        else {
            throw new ArgumentException($"Modifier of type {modifier.GetType()} is not supported.");
        }
    }

    public void Remove(IStatModifier<T> modifier) {
        if (modifier is SetterStatModifier<T> setter) {
            _modifiers.Remove(setter);
        }
    }

    public void RemoveAll(string sourceId) => _modifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);

    public void Clear() => _modifiers.Clear();
}


public abstract class NumericsStatModifiers<T,TFlat, TPercent, TClamp> : IStatModifiers<T> 
    where TFlat : IFlatModifier<T> 
    where TPercent : IPercentageModifier<T>
    where TClamp : IClampModifier<T> {

    private HashSet<IStatModifier<T>> _setters = new();
    private HashSet<IStatModifier<T>> _flatModifiers = new();
    private HashSet<IStatModifier<T>> _percentageModifiers = new();
    private HashSet<IStatModifier<T>> _positiveClampers = new();
    private HashSet<IStatModifier<T>> _negativeClampers = new();

    public T ModifyValue(T sourceValue) {
        sourceValue = _setters.Count > 0 ? _setters.Max(modifier => modifier.ModifyValue(sourceValue)) : sourceValue;
        foreach (var fmod in _flatModifiers) sourceValue = fmod.ModifyValue(sourceValue);
        foreach (var pmod in _percentageModifiers) sourceValue = pmod.ModifyValue(sourceValue);
        foreach (var cpmod in _positiveClampers) sourceValue = cpmod.ModifyValue(sourceValue);
        foreach (var cnmod in _negativeClampers) sourceValue = cnmod.ModifyValue(sourceValue);
        return sourceValue;
    }

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

    public void RemoveAll(string sourceId) {
        _setters.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _flatModifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _percentageModifiers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _positiveClampers.RemoveWhere(modifier => modifier.SourceId == sourceId);
        _negativeClampers.RemoveWhere(modifier => modifier.SourceId == sourceId);
    }

    public void Clear() {
        _setters.Clear();
        _flatModifiers.Clear();
        _percentageModifiers.Clear();
        _positiveClampers.Clear();
        _negativeClampers.Clear();
    }
}
public class IntStatModifiers : NumericsStatModifiers<int, IntFlatStatModifier, IntPercentageStatModifier, IntClampStatModifier> {}

public class FloatStatModifiers : NumericsStatModifiers<float, FloatFlatStatModifier, FloatPercentageStatModifier, FloatClampStatModifier> { }

public static class StatModifiersFactory {
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