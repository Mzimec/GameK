using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base interface for all stats collections.
/// </summary>
/// <typeparam name="TStat">Type of the stat that works with value type TValue</typeparam>
/// <typeparam name="TValue">Value tzpe of the stat</typeparam>
public interface IStats<TStat, TValue> where TStat : IStat<TValue> {
    /// <summary>
    /// Collection of all stats in this stats collection.
    /// </summary>
    public IEnumerable<TStat> Stats { get; }
}

/// <summary>
/// An interface for components that belong to an owner of type TOwner.
/// </summary>
/// <typeparam name="TOwner">The type of the owner.</typeparam>
/// <typeparam name="TStat">Type of the stat that works with value type TValue</typeparam>
/// <typeparam name="TValue">Value type of the stat</typeparam>
public interface IOwnedStats<TStat, TValue, TOwner> : IStats<TStat, TValue>, IComponent<TOwner>
    where TStat : IStat<TValue>
    where TOwner : IHasStats {
}


/// <summary>
/// Represents the core attributes of a character, including Strength, Dexterity, Constitution, Intelligence, Wisdom, and Charisma.
/// </summary>
public class  CharacterAttributes : IOwnedStats<IntComparableStat, int, CharacterCore> {

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterAttributes"/> class and associates it with the specified owner.
    /// </summary>
    /// <param name="owner">Owner of these attributes</param>
    /// <exception cref="ArgumentNullException">For reason where owner is null</exception>
    public CharacterAttributes(CharacterCore owner) {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _owner.Stats.AddStats(Stats);
    }

    private CharacterCore _owner;

    /// <summary>
    /// Gets the owner of these attributes.
    /// </summary>
    public CharacterCore Owner => _owner;

    /// <summary>
    /// Gets the Strength attribute, representing physical power and carrying capacity.
    /// </summary>
    public IntComparableStat Strength { get; private set; }

    /// <summary>
    /// Gets the Dexterity attribute, representing agility, reflexes, and balance.
    /// </summary>
    public IntComparableStat Dexterity { get; private set; }

    /// <summary>
    /// Gets the Constitution attribute, representing endurance, health, and stamina.
    /// </summary>
    public IntComparableStat Constitution { get; private set; }

    /// <summary>
    /// Gets the Intelligence attribute, representing reasoning, memory, and problem-solving abilities.
    /// </summary>
    public IntComparableStat Intelligence { get; private set; }

    /// <summary>
    /// Gets the Wisdom attribute, representing perception, insight, and intuition.
    /// </summary>
    public IntComparableStat Wisdom { get; private set; }

    /// <summary>
    /// Gets the Charisma attribute, representing force of personality, persuasiveness, and leadership.
    /// </summary>
    public IntComparableStat Charisma { get; private set; }

    /// <inheritdoc/>
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



