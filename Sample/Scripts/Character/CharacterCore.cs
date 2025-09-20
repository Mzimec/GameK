using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public interface IEntity {
    string Id { get; }
    Guid GuID { get; }
}

public interface IDamagable { }

public interface IDamagable<TReceiver, TOwner> : IDamagable
    where TReceiver : IDamageReceiver<TOwner> 
    where TOwner : IHasEventBus {
    public TReceiver DamageReceiver { get; }
}

public interface IHasAttributes {
    public CharacterAttributes Attributes { get; } 
}

public interface IHasVitals<TVitals>
    where TVitals : IVitals {
    public TVitals Vitals { get; }
}

public interface IHasResistances { }

public interface IHasEventBus {
    public IEventBus EventBus { get; }
}

public interface  IHasInventory {
    // public IInvenotry
}

public interface IHasAbilities {
    // public IAbilities abilities { get; }
}

public interface IHasAI {
    // public IAI ai { get; }
}

public interface IHasEffects { 
}

public interface IHasStats { 
    public IStatsWrapper Stats { get; }
}





public class CharacterCore :
    IHasStats,
    IHasAttributes,
    IHasVitals<CharacterVitals> {

    public string Id { get; }
    //public CharacterDefinition Definition { get; }
    //public CharacterData SaveData { get; }

    public IStatsWrapper Stats { get; }
    public CharacterAttributes Attributes { get; }
    public CharacterVitals Vitals { get; }
    //public CharacterEffects Effects { get; }
    //public CharacterInventory Inventory { get; }
    //public CharacterAbilities Abilities { get; }
    //public CharacterAI AI { get; }
    public BaseEventBus EventBus { get; }
    //public CharacterDamageReceiver DamageReceiver { get; }

    /*public CharacterCore(CharacterDefinition definition, CharacterData? data = null) {
        // Injekce definice nebo load ze save
    }*/
}
