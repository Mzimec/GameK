using System;
using System.Collections.Generic;
using UnityEngine;

public enum EStatType {
    Stat,
    ModifiableStat,
    ResourceStat,
}

public enum EStatValueType {
    Int,
    Float,
    Bool,
}

[CreateAssetMenu(menuName = "Stats/StatType")]
public class StatTypeSO : ScriptableObject {
    [SerializeField] private string _statName;
    [SerializeField] private string _description;

    [SerializeField, HideInInspector] private EStatType _statType;
    [SerializeField, HideInInspector] private EStatValueType _valueType;

    private Type _cachedType;
    private Type _valueCachedType;

    public string StatName  { get => _statName; set => _statName = value; }
    public string Description { get => _description; set => _description = value; }

    public Type StatInterfaceType => _cachedType ??= ResolveStatInterfaceType();

    private Type ValueType => _valueCachedType ??= _valueType switch
    {
        EStatValueType.Int => typeof(int),
        EStatValueType.Float => typeof(float),
        EStatValueType.Bool => typeof(bool),
        _ => throw new NotSupportedException($"Unsupported value type: {_valueType}")
    };

    private Type ResolveStatInterfaceType() {
        var valueType = ValueType;

        return _statType switch
        {
            EStatType.Stat => typeof(IStat<>).MakeGenericType(valueType),
            EStatType.ModifiableStat => typeof(IModifiableStat<>).MakeGenericType(valueType),
            EStatType.ResourceStat => typeof(IResourceStat<>).MakeGenericType(valueType),
            _ => throw new NotSupportedException($"Unsupported stat type: {_statType}")
        };
    }

    private void OnEnable() {
        _ = StatInterfaceType;
    }
}