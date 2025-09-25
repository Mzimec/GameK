using System.Collections.Generic;
using UnityEngine;

public class DealDamageAction : ActionNode {
    private int damageAmount;
    private EDamageType damageType;

    protected override void SelfExecute(ActionContext ctx) {
        if (ctx.Target is IDamageReceiver damageReceiver) {
            var damageByType = new Dictionary<EDamageType, int> {
                { damageType, damageAmount }
            };
            var damageContext = new DamageContext
            {
                Source = ctx.Source,
                Target = damageReceiver,
                DamageByType = damageByType,
                TotalDamage = damageAmount
            };
            damageReceiver.ReceiveDamage(damageContext);
        }
        else {
            Debug.LogWarning("Target does not implement IDamageReceiver.");
        }
    }

    protected override ActionApproximation ApproximateSelfExecute(ActionContext ctx) {
        if (ctx.Target is IDamageReceiver damageReceiver) {
            var expectedDamage = damageReceiver.ModifyDamage(new DamageContext
            {
                Source = ctx.Source,
                Target = damageReceiver,
                DamageByType = new Dictionary<EDamageType, int>
                {
                    { damageType, damageAmount }
                },
                TotalDamage = damageAmount
            }).TotalDamage;
            ActionApproximation approximation = new();
            approximation.DamageDealt[ctx.Target] = expectedDamage;
            return approximation;
        }
        else return new ActionApproximation();
    }
}