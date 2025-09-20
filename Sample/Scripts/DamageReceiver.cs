using System.ComponentModel.Design;
using UnityEngine;


public struct DamagePayload { }

public interface IDamageReceiver<T> 
    where T : IHasEventBus {
    public T Owner { get; }
    public void ReceiveDamage(int damage, IHasAbilities source) {
        //Owner.EventBus
    }

    public int ModifyDamage(DamagePayload damage) {
        // Modify damage based on stats or effects
        // Example: damage = damage * Owner.Stats.DamageMultiplier;
        return 0;
    }

}

/*public class CharacterDamageReceiver : IDamageReceiver<CharacterCore> {

    private CharacterCore _owner;
    public CharacterDamageReceiver(CharacterCore owner) => this._owner = owner;
    public CharacterCore Owner => _owner;
}*/
