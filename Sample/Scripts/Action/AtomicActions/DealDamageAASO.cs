using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DealDamageAASO",
    menuName = "Scriptable Objects/Atomic Actions/DealDamage"
)]
public class DealDamageAASO : AtomicActionSO {
    [SerializeField] private int baseDamage;
    [SerializeField] private List<StatScalingEntry> statScaling = new List<StatScalingEntry>();

    public override void Execute(ActionContext ctx, Vector3Int target) {
        if (ctx.Source == null) {
            Debug.LogWarning("DealDamageAASO: Source is null.");
            return;
        }

        int damageAmount = baseDamage;

        if (statScaling.Count != 0) {
            foreach (var scalingPair in statScaling) {
                var statType = scalingPair.StatType;
                var multiplier = scalingPair.Multiplier;
                var stat = ctx.Source.Stats.GetStat(statType) as IStat<int>;
                if (stat != null) {
                    damageAmount += Mathf.RoundToInt(stat.Value * multiplier);
                }
                else {
                    Debug.LogWarning($"DealDamageAASO: Stat {statType.StatName} not found or not of type int.");
                }
            }
        }

        /*if (ctx.Target is IDamagable damagable) {
            damagable.TakeDamage(damageAmount, ctx.Source);
            Debug.Log($"DealDamageAASO: {ctx.Source.name} dealt {damageAmount} damage to {ctx.Target.name}.");
        }
        else {
            Debug.LogWarning("DealDamageAASO: Target is not IDamagable.");
        }*/
    }
}


