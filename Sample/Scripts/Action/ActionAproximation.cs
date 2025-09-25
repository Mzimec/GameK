using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an approximation of the effects of an action, including damage, healing, applied effects, and kill chances.
/// </summary>
public class ActionApproximation {
    /// <summary>
    /// Maps each target to the total damage dealt.
    /// </summary>
    public Dictionary<ITargetable, int> DamageDealt = new();

    /// <summary>
    /// Maps each target to the total healing done.
    /// </summary>
    public Dictionary<ITargetable, int> HealingDone = new();

    /// <summary>
    /// Maps each target to a dictionary of effects and their application chances.
    /// </summary>
    public Dictionary<ITargetable, Dictionary<IEffect, float>> EffectsApplied = new();

    /// <summary>
    /// Maps each target to the chance that the character was killed (max 1.0).
    /// </summary>
    public Dictionary<ITargetable, float> CharactersKilled = new();

    /// <summary>
    /// Merges the results from another <see cref="ActionApproximation"/> into this instance.
    /// Sums values and clamps effect/kill chances to a maximum of 1.0.
    /// </summary>
    /// <param name="other">The other <see cref="ActionApproximation"/> to merge.</param>
    public void Merge(ActionApproximation other) {
        foreach (var (target, damage) in other.DamageDealt) {
            if (DamageDealt.ContainsKey(target)) DamageDealt[target] += damage;
            else DamageDealt[target] = damage;
        }
        foreach (var (target, healing) in other.HealingDone) {
            if (HealingDone.ContainsKey(target)) HealingDone[target] += healing;
            else HealingDone[target] = healing;
        }
        foreach (var (target, effects) in other.EffectsApplied) {
            if (!EffectsApplied.ContainsKey(target)) EffectsApplied[target] = new Dictionary<IEffect, float>();
            foreach (var (effect, chance) in effects) {
                if (EffectsApplied[target].ContainsKey(effect)) EffectsApplied[target][effect] += chance;
                else EffectsApplied[target][effect] = chance;
                if (EffectsApplied[target][effect] > 1f) EffectsApplied[target][effect] = 1f;
            }
        }
        foreach (var (target, chance) in other.CharactersKilled) {
            if (CharactersKilled.ContainsKey(target)) CharactersKilled[target] += chance;
            else CharactersKilled[target] = chance;
            if (CharactersKilled[target] > 1f) CharactersKilled[target] = 1f;
        }
    }

    /// <summary>
    /// Merges the results from another <see cref="ActionApproximation"/> into this instance,
    /// scaling all values by the given coefficient <paramref name="c"/>.
    /// Sums values and clamps effect/kill chances to a maximum of 1.0.
    /// </summary>
    /// <param name="other">The other <see cref="ActionApproximation"/> to merge.</param>
    /// <param name="c">The scaling coefficient to apply to all values.</param>
    public void Merge(ActionApproximation other, float c) {
        foreach (var (target, damage) in other.DamageDealt) {
            if (DamageDealt.ContainsKey(target)) DamageDealt[target] += (int)(damage * c);
            else DamageDealt[target] = (int)(damage * c);
        }
        foreach (var (target, healing) in other.HealingDone) {
            if (HealingDone.ContainsKey(target)) HealingDone[target] += (int)(healing * c);
            else HealingDone[target] = (int)(healing * c);
        }
        foreach (var (target, effects) in other.EffectsApplied) {
            if (!EffectsApplied.ContainsKey(target)) EffectsApplied[target] = new Dictionary<IEffect, float>();
            foreach (var (effect, chance) in effects) {
                if (EffectsApplied[target].ContainsKey(effect)) EffectsApplied[target][effect] += chance * c;
                else EffectsApplied[target][effect] = chance * c;
                if (EffectsApplied[target][effect] > 1f) EffectsApplied[target][effect] = 1f;
            }
        }
        foreach (var (target, chance) in other.CharactersKilled) {
            if (CharactersKilled.ContainsKey(target)) CharactersKilled[target] += chance * c;
            else CharactersKilled[target] = chance * c;
            if (CharactersKilled[target] > 1f) CharactersKilled[target] = 1f;
        }
    }
}
