using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specifies the type of stat, such as basic, modifiable, or resource.
/// </summary>
public enum EStatType {
    Stat,
    ModifiableStat,
    ResourceStat,
}

/// <summary>
/// Specifies the value type of a stat, such as int, float, or bool.
/// </summary>
public enum EStatValueType {
    Int,
    Float,
    Bool,
}

/// <summary>
/// Specifies the category of a stat, such as Armor Class or Attack Bonus.
/// </summary>
public enum EStatCategory {
    AC,
    AttackBonus,
}

/// <summary>
/// ScriptableObject representing the definition and metadata for a stat type.
/// </summary>
[CreateAssetMenu(menuName = "Stats/StatType")]
public class StatTypeSO : ScriptableObject {
    [SerializeField] private string _statName;
    [SerializeField] private string _description;
    /// <summary>
    /// The category of the stat.
    /// </summary>
    [SerializeField] public EStatCategory StatCategory;

    [SerializeField, HideInInspector] private EStatType _statType;
    [SerializeField, HideInInspector] private EStatValueType _valueType;

    private Type _cachedType;
    private Type _valueCachedType;

    /// <summary>
    /// The name of the stat.
    /// </summary>
    public string StatName { get => _statName; set => _statName = value; }

    /// <summary>
    /// The description of the stat.
    /// </summary>
    public string Description { get => _description; set => _description = value; }

    /// <summary>
    /// Gets the .NET type representing the stat interface for this stat type.
    /// </summary>
    public Type StatInterfaceType => _cachedType ??= ResolveStatInterfaceType();

    /// <summary>
    /// Gets the .NET type representing the value type of the stat.
    /// </summary>
    private Type ValueType => _valueCachedType ??= _valueType switch
    {
        EStatValueType.Int => typeof(int),
        EStatValueType.Float => typeof(float),
        EStatValueType.Bool => typeof(bool),
        _ => throw new NotSupportedException($"Unsupported value type: {_valueType}")
    };

    /// <summary>
    /// Resolves the stat interface type based on the stat type and value type.
    /// </summary>
    /// <returns>The .NET type of the stat interface.</returns>
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

    /// <summary>
    /// Called by Unity when the object is enabled. Initializes the stat interface type cache.
    /// </summary>
    private void OnEnable() {
        _ = StatInterfaceType;
    }
}
