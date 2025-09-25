using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iterface for manging all different stats an entity might have.
/// </summary>
public interface IStatsWrapper {

    /// <summary>
    /// Gets a read-only dictionary of all stats, keyed by their stat category.
    /// </summary>
    public IReadOnlyDictionary<EStatCategory, IStatBase> Stats { get; }

    /// <summary>
    /// Adds multiple stats to the stats wrapper.
    /// </summary>
    /// <param name="stats">Collection of the stats to add</param>
    public void AddStats(IEnumerable<IStatBase> stats);

    /// <summary>
    /// Adds a single stat to the stats wrapper.
    /// </summary>
    /// <param name="stat">The stat to add</param>
    public void AddStats(IStatBase stat);

    /// <summary>
    /// Removes multiple stats from the stats wrapper.
    /// </summary>
    /// <param name="stats">Collection of the stats to remove</param>
    public void RemoveStats(IEnumerable<IStatBase> stats);

    /// <summary>
    /// Removes a single stat from the stats wrapper.
    /// </summary>
    /// <param name="stat">The stat to remove</param>
    public void RemoveStats(IStatBase stat);

    /// <summary>
    /// Gets a stat by its category.
    /// </summary>
    /// <param name="statName">The category of the stat to retrieve</param>
    public IStatBase GetStat(EStatCategory statName);
}


/// <summary>
/// Concrete implementation of <see cref="IStatsWrapper"/> for managing entity stats.
/// </summary>
public class StatsWrapper : IStatsWrapper
{
    private Dictionary<EStatCategory, IStatBase> _stats = new Dictionary<EStatCategory, IStatBase>();

    /// <inheritdoc/>
    public IReadOnlyDictionary<EStatCategory, IStatBase> Stats => _stats;

    /// <inheritdoc/>
    public void AddStats(IStatBase stat) {
        if (!_stats.ContainsKey(stat.StatType.StatCategory)) {
            _stats.Add(stat.StatType.StatCategory, stat);
        }
    }

    /// <inheritdoc/>
    public void AddStats(IEnumerable<IStatBase> stats) {
        foreach (var stat in stats) {
            AddStats(stat);
        }
    }

    /// <inheritdoc/>
    public void RemoveStats(IStatBase stat) {
        if (_stats.ContainsKey(stat.StatType.StatCategory)) {
            _stats.Remove(stat.StatType.StatCategory);
        }
    }

    /// <inheritdoc/>
    public void RemoveStats(IEnumerable<IStatBase> stats) {
        foreach (var stat in stats) {
            RemoveStats(stat);
        }
    }

    /// <inheritdoc/>
    public IStatBase GetStat(EStatCategory statType) {
        _stats.TryGetValue(statType, out var stat);
        return stat;
    }
}
