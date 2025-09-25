using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

/// <summary>
/// Represents an entity with a unique string ID and GUID.
/// </summary>
public interface IEntity {
    string Id { get; }
    Guid GuID { get; }
}



/// <summary>
/// Interface for objects that can take damage and have a damage receiver.
/// </summary>
/// <typeparam name="TReceiver">Type of the damage receiver.</typeparam>
/// <typeparam name="TOwner">Type of the owner with an event bus.</typeparam>
public interface IDamagable {
    public IDamageReceiver DamageReceiver { get; }
}

/// <summary>
/// Interface for objects that have character attributes.
/// </summary>
public interface IHasAttributes {
    public CharacterAttributes Attributes { get; }
}

/// <summary>
/// Interface for objects that have vitals.
/// </summary>
/// <typeparam name="TVitals">Type of the vitals.</typeparam>
public interface IHasVitals {
    public IVitals Vitals { get; }
}

/// <summary>
/// Marker interface for objects that have resistances.
/// </summary>
public interface IHasResistances { }

/// <summary>
/// Interface for objects that have an event bus.
/// </summary>
public interface IHasEventBus {
    public IEventBus EventBus { get; }
}

/// <summary>
/// Marker interface for objects that have an inventory.
/// </summary>
public interface IHasInventory {
    // public IInvenotry
}

/// <summary>
/// Marker interface for objects that have abilities.
/// </summary>
public interface IHasAbilities {
    // public IAbilities abilities { get; }
}

/// <summary>
/// Marker interface for objects that have AI.
/// </summary>
public interface IHasAI {
    public ICharacterAI CharacterAI { get; }
}

/// <summary>
/// Marker interface for objects that have effects.
/// </summary>
public interface IHasEffects {
}

/// <summary>
/// Interface for objects that have stats.
/// </summary>
public interface IHasStats {
    public IStatsWrapper Stats { get; }
}

/// <summary>
/// Interface for objects that have actions.
/// </summary>
public interface IHasActions {
    public IActions Actions { get; }
}

/// <summary>
/// Core class representing a character, including stats, attributes, vitals, and actions.
/// </summary>
public class CharacterCore :
    IHasStats,
    IHasAttributes,
    IHasVitals,
    IHasActions,
    IDamagable,
    IHasAI{

    /// <summary>
    /// Unique string identifier for the character.
    /// </summary>
    public string Id { get; }
    //public CharacterDefinition Definition { get; }
    //public CharacterData SaveData { get; }

    /// <summary>
    /// Wrapper for character stats.
    /// </summary>
    public IStatsWrapper Stats { get; }

    /// <summary>
    /// Character attributes such as strength, dexterity, etc.
    /// </summary>
    public CharacterAttributes Attributes { get; }

    /// <summary>
    /// Character vitals such as health and regeneration.
    /// </summary>
    public IVitals Vitals { get; }

    /// <summary>
    /// Actions available to the character.
    /// </summary>
    public IActions Actions { get; }
    public IDamageReceiver DamageReceiver { get; }
    public ICharacterAI CharacterAI { get; }
    //public CharacterEffects Effects { get; }
    //public CharacterInventory Inventory { get; }
    //public CharacterAbilities Abilities { get; }
    //public CharacterAI AI { get; }

    /// <summary>
    /// Event bus for character events.
    /// </summary>
    public BaseEventBus EventBus { get; }
    //public CharacterDamageReceiver DamageReceiver { get; }

    /*public CharacterCore(CharacterDefinition definition, CharacterData? data = null) {
        // Injekce definice nebo load ze save
    }*/
}
