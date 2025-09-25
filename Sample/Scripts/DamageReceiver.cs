using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public enum EDamageType {
    Physical,
    Magical,
    True
}

public struct DamagePayload { }

public struct HealingContext { 
    public CharacterCore Source;
    public IDamageReceiver Target;
    public int HealingAmount;
}

public struct DamageContext {
    public CharacterCore Source;
    public IDamageReceiver Target;
    public Dictionary<EDamageType, int> DamageByType;
    public int TotalDamage;
}

public interface IDamageReceiver {
    public void ReceiveDamage(DamageContext ctx);

    public DamageContext ModifyDamage(DamageContext ctx);

    public void ReceiveHealing(HealingContext ctx);
}

public class GenericDamageReceiver<TOwner> : IDamageReceiver, IComponent<TOwner>
    where TOwner : IHasVitals, IHasEventBus {

    private TOwner _owner;
    public TOwner Owner => _owner;

    private Dictionary<EDamageType, IModifiableStat<int>> _damageReductions;
    public void ReceiveDamage(DamageContext ctx) {
        var modifiedCtx = ModifyDamage(ctx);
        if (modifiedCtx.TotalDamage <= 0) return;
        _owner.EventBus.Publish(new DamageTakenEvent(modifiedCtx));
        modifiedCtx.Source.EventBus.Publish(new DamageDealtEvent(modifiedCtx));
        _owner.Vitals.TakeDamage(modifiedCtx.TotalDamage);
    }
    public DamageContext ModifyDamage(DamageContext ctx) {
        int totalDamage = 0;
        DamageContext modifiedCtx = new();
        foreach (var (damageType, damage) in ctx.DamageByType) {
            modifiedCtx.DamageByType[damageType] = damage - _damageReductions[damageType].Value;
            if (modifiedCtx.DamageByType[damageType] < 0) modifiedCtx.DamageByType[damageType] = 0;
            totalDamage += modifiedCtx.DamageByType[damageType];
        }
        modifiedCtx.Source = ctx.Source;
        modifiedCtx.Target = ctx.Target;
        modifiedCtx.TotalDamage = totalDamage;
        return modifiedCtx;
    }
    public void ReceiveHealing(HealingContext ctx) {
        var modifiedCtx = ModifyHealing(ctx);
        if (modifiedCtx.HealingAmount <= 0) return;
        _owner.EventBus.Publish(new HealedEvent(modifiedCtx));
        modifiedCtx.Source.EventBus.Publish(new HealingDoneEvent(modifiedCtx));
        _owner.Vitals.TakeHeal(modifiedCtx.HealingAmount);
    }

    public HealingContext ModifyHealing(HealingContext ctx) {
        if (_owner.Vitals.Health.CurrentValue + ctx.HealingAmount > _owner.Vitals.Health.Value) {
            ctx.HealingAmount = _owner.Vitals.Health.Value - _owner.Vitals.Health.CurrentValue;
        }
        return ctx;
    }
}

/*public class CharacterDamageReceiver : IDamageReceiver<CharacterCore> {

    private CharacterCore _owner;
    public CharacterDamageReceiver(CharacterCore owner) => this._owner = owner;
    public CharacterCore Owner => _owner;
}*/
