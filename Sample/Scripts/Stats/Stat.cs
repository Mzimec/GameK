using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStatBase { 
    public StatTypeSO StatType { get; }
}

public interface IStat<T> : IStatBase {
    public T Value { get; }
}

public interface IModifiableStat<T> : IStat<T> { 
    public void AddModifier(IStatModifier<T> modifier);
    public void RemoveModifier(IStatModifier<T> modifier);
    public void RemoveAllModifiers(string sourceId);
    public void ResetValue();
}

public interface IResourceStat<T> : IModifiableStat<T> {
    public T CurrentValue { get; }
    public void ModifyCurrentValue(T value);
}

public interface IRegeneratable<T, TStat>
    where TStat : IStat<T> {
    public TStat Regeneration { get; }
    public void Regenerate();
}

public class ModifiableStat<T> : IModifiableStat<T> {
    private T _baseValue;
    protected virtual T _value { get; set; }
    private IStatModifiers<T> _modifiers;

    public ModifiableStat(T baseValue, StatTypeSO tStat) {
        _baseValue = baseValue;
        _modifiers = StatModifiersFactory.Create<T>();
        _value = CalculateValue();
        StatType = tStat;
    }

    public StatTypeSO StatType { get; }

    private T CalculateValue() => _modifiers.ModifyValue(BaseValue);

    public T BaseValue { 
        get => _baseValue;
        set {
            _baseValue = value;
            _value = CalculateValue();
        }
    }
    public virtual T Value => _value; 
    public void AddModifier(IStatModifier<T> modifier) {
        _modifiers.Add(modifier);
        _value = CalculateValue();
    }
    public void RemoveModifier(IStatModifier<T> modifier) {
        _modifiers.Remove(modifier);
        _value = CalculateValue();
    }
    public void RemoveAllModifiers(string sourceId) {
        _modifiers.RemoveAll(sourceId);
        _value = CalculateValue();
    }
    public void ResetValue() { 
        _modifiers.Clear();
        _value = CalculateValue();
    }
}

public class ComparableModifiableStat<T> : ModifiableStat<T>, IComparable<ComparableModifiableStat<T>>
    where T : IComparable<T> {

    private readonly T _minValue;
    private readonly T _maxValue;
    public ComparableModifiableStat(T baseValue, StatTypeSO tStat, T minValue, T maxValue) : base(baseValue,tStat) {
        _minValue = minValue;
        _maxValue = maxValue;
    }
    public int CompareTo(ComparableModifiableStat<T> other) {
        if (other == null) return 1;
        return Value.CompareTo(other.Value);
    }

    public override T Value {
        get {
            var val = base.Value;
            if (val.CompareTo(_minValue) < 0) return _minValue;
            if (val.CompareTo(_maxValue) > 0) return _maxValue;
            return val;
        }
    }
}

public abstract class AbstractResourceStat<T> : ModifiableStat<T>, IResourceStat<T> {
    protected T _currentValue;
    protected override T _value { get => base._value; set {
            base._value = value;
            _currentValue = Value;
        }
    }

    public AbstractResourceStat(T baseValue, StatTypeSO tStat) : base(baseValue, tStat) {
        _currentValue = baseValue;
    }
    public T CurrentValue => _currentValue;
    public abstract void ModifyCurrentValue(T value);
}

public class IntResourceStat : AbstractResourceStat<int> {
    protected override int _value {
        get => base._value; set {
            base._value = value;
            _currentValue = Mathf.Clamp(_currentValue, 0, Value);
        }
    }
    public IntResourceStat(int baseValue, StatTypeSO tStat) : base(baseValue, tStat) { }
    public override void ModifyCurrentValue(int value) => _currentValue = Mathf.Clamp(_currentValue + value, 0, Value);
}

public class IntComparableStat : ComparableModifiableStat<int> {
    public IntComparableStat(int baseValue, StatTypeSO tStat, int minValue, int maxValue) : base(baseValue, tStat, minValue, maxValue) { }
}


