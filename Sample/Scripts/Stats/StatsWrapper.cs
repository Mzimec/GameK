using System.Collections.Generic;
using UnityEngine;

public interface IStatsWrapper {
    public IReadOnlyDictionary<StatTypeSO, IStatBase> Stats { get; }
    public void AddStats(IEnumerable<IStatBase> stats);
    public void AddStats(IStatBase stat);
    public void RemoveStats(IEnumerable<IStatBase> stats);
    public void RemoveStats(IStatBase stat);
    public IStatBase GetStat(StatTypeSO statType);
}

public class StatsWrapper : IStatsWrapper
{
    private Dictionary<StatTypeSO, IStatBase> _stats = new Dictionary<StatTypeSO, IStatBase>();
    public IReadOnlyDictionary<StatTypeSO, IStatBase> Stats => _stats;

    public void AddStats(IStatBase stat) {
        if (!_stats.ContainsKey(stat.StatType)) {
            _stats.Add(stat.StatType, stat);
        }
    }

    public void AddStats(IEnumerable<IStatBase> stats) {
        foreach (var stat in stats) {
            AddStats(stat);
        }
    }

    public void RemoveStats(IStatBase stat) {
        if (_stats.ContainsKey(stat.StatType)) {
            _stats.Remove(stat.StatType);
        }
    }

    public void RemoveStats(IEnumerable<IStatBase> stats) {
        foreach (var stat in stats) {
            RemoveStats(stat);
        }
    }

    public IStatBase GetStat(StatTypeSO statType) {
        _stats.TryGetValue(statType, out var stat);
        return stat;
    }
}
