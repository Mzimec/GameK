using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStats<TStat, TValue> where TStat : IStat<TValue> {
    public IEnumerable<TStat> Stats { get; }
}

public interface IOwnedStats<TStat, TValue, TOwner> : IStats<TStat, TValue>, IComponent<TOwner>
    where TStat : IStat<TValue>
    where TOwner : IHasStats {
}

public class  CharacterAttributes : IOwnedStats<IntComparableStat, int, CharacterCore> {

    public CharacterAttributes(CharacterCore owner) {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _owner.Stats.AddStats(Stats);
    }

    private CharacterCore _owner;
    public CharacterCore Owner => _owner;

    public IntComparableStat Strength { get; private set; }
    public IntComparableStat Dexterity { get; private set; }
    public IntComparableStat Constitution { get; private set; }
    public IntComparableStat Intelligence { get; private set; }
    public IntComparableStat Wisdom { get; private set; }
    public IntComparableStat Charisma { get; private set; }

    public IEnumerable<IntComparableStat> Stats { get {
            yield return Strength;
            yield return Dexterity;
            yield return Constitution;
            yield return Intelligence;
            yield return Wisdom;
            yield return Charisma;
        }
    }
}



