
/// <summary>
/// Marker interface for all event types in the system.
/// </summary>
public interface IEvent {
    // No members; used for type safety and event identification.
}


/// <summary>
/// Marker interface for all event types that are called on a Character.
/// </summary>
public interface ICharacterEvent : IEvent { }

/// <summary>
/// Marker interface for all event types that are called during Actions and have to react on it.
/// </summary>
public interface IActionEvent : IEvent {
    public ActionContext ActionContext { get; }
}


/// <summary>
/// A test event used for demonstration purposes.
/// </summary>
public struct TestEvent : IEvent { }


/// <summary>
/// A test event used for demonstration purposes that carries player health and mana data.
///</summary>
public struct PlayerEvent : IEvent {
    public int health;
    public int mana;
}

public struct DamageTakenEvent : ICharacterEvent { 
    public DamageContext DamageContext;

    public DamageTakenEvent(DamageContext damageContext) {
        DamageContext = damageContext;
    }
}

public struct DamageDealtEvent : ICharacterEvent {
    public DamageContext DamageContext;
    public DamageDealtEvent(DamageContext damageContext) {
        DamageContext = damageContext;
    }
}

public struct HealedEvent : ICharacterEvent {
    public HealingContext HealingContext;
    public HealedEvent(HealingContext healingContext) {
        HealingContext = healingContext;
    }
}

public struct HealingDoneEvent : ICharacterEvent {
    public HealingContext HealingContext;
    public HealingDoneEvent(HealingContext healingContext) {
        HealingContext = healingContext;
    }
}
