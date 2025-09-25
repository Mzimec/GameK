using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for resolving roll outcomes and calculating success chances.
/// </summary>
public interface IRollResolver {
    /// <summary>
    /// Resolves a roll based on the provided action context.
    /// </summary>
    /// <param name="ctx">The action context for the roll.</param>
    /// <returns>A <see cref="RollRecord"/> containing the result of the roll.</returns>
    RollRecord ResolveRoll(ActionContext ctx);

    /// <summary>
    /// Calculates the success chance for each possible roll result.
    /// </summary>
    /// <param name="roll">The roll record to evaluate.</param>
    /// <returns>
    /// A dictionary mapping <see cref="ERollResult"/> to their respective success probabilities.
    /// </returns>
    Dictionary<ERollResult, float> SuccessChance(RollRecord roll);
}

/// <summary>
/// Represents the possible outcomes of a roll.
/// </summary>
public enum ERollResult {
    /// <summary>
    /// The roll resulted in a critical failure (e.g., rolling a 1).
    /// </summary>
    CriticalFailure,
    /// <summary>
    /// The roll failed to meet the target number.
    /// </summary>
    Failure,
    /// <summary>
    /// The roll succeeded in meeting or exceeding the target number.
    /// </summary>
    Success,
    /// <summary>
    /// The roll resulted in a critical success (e.g., rolling a 20).
    /// </summary>
    CriticalSuccess,
    /// <summary>
    /// No result has been determined yet.
    /// </summary>
    None
}

/// <summary>
/// Contains the data and result of a single roll.
/// </summary>
public struct RollRecord {
    /// <summary>
    /// The value rolled (typically 1-20).
    /// </summary>
    public int Roll { get; }

    /// <summary>
    /// The target number to beat for success.
    /// </summary>
    public int TargetNumber { get; }

    /// <summary>
    /// The bonus applied to the roll.
    /// </summary>
    public int RollBonus { get; }

    /// <summary>
    /// A log message describing the roll and its result.
    /// </summary>
    public string Log => $"Roll of {Roll} against target {TargetNumber} resulted in {Result}";

    /// <summary>
    /// The result of the roll.
    /// </summary>
    public ERollResult Result { get; set; }

    /// <summary>
    /// The chance of success, calculated as a float between 0 and 1.
    /// </summary>
    public float SuccessChance => (Roll - 1) / 19f;

    /// <summary>
    /// Initializes a new instance of <see cref="RollRecord"/>.
    /// </summary>
    /// <param name="roll">The rolled value.</param>
    /// <param name="targetNumber">The target number to beat.</param>
    /// <param name="rollBonus">The bonus applied to the roll.</param>
    public RollRecord(int roll, int targetNumber, int rollBonus) {
        Roll = roll;
        TargetNumber = targetNumber;
        RollBonus = rollBonus;
        Result = ERollResult.None;
        DetermineResult();
    }

    /// <summary>
    /// Determines the result of the roll based on its value and the target number.
    /// </summary>
    private void DetermineResult() {
        switch (Roll) {
            case 1:
                Result = ERollResult.CriticalFailure;
                break;
            case 20:
                Result = ERollResult.CriticalSuccess;
                break;
            default:
                if (Roll >= TargetNumber - RollBonus) {
                    Result = ERollResult.Success;
                }
                else {
                    Result = ERollResult.Failure;
                }
                break;
        }
    }
}

/// <summary>
/// Base class for roll resolvers, providing default success chance calculation.
/// </summary>
public abstract class BaseRollResolver : IRollResolver {
    /// <summary>
    /// Resolves a roll based on the provided action context.
    /// </summary>
    /// <param name="ctx">The action context for the roll.</param>
    /// <returns>A <see cref="RollRecord"/> containing the result of the roll.</returns>
    public abstract RollRecord ResolveRoll(ActionContext ctx);

    /// <summary>
    /// Calculates the success chance for each possible roll result.
    /// </summary>
    /// <param name="roll">The roll record to evaluate.</param>
    /// <returns>
    /// A dictionary mapping <see cref="ERollResult"/> to their respective success probabilities.
    /// </returns>
    public virtual Dictionary<ERollResult, float> SuccessChance(RollRecord roll) => new()
    {
        { ERollResult.CriticalFailure, 0.05f },
        { ERollResult.Failure, ((roll.TargetNumber - roll.RollBonus - 1) / 20f) - 0.05f },
        { ERollResult.Success, ((21 - roll.TargetNumber + roll.RollBonus) / 20f) - 0.05f },
        { ERollResult.CriticalSuccess, 0.05f },
    };
}

/// <summary>
/// Roll resolver for saving throws, using a difficulty class and stat category.
/// </summary>
public class SaveRollData : BaseRollResolver {
    private readonly int difficultyClass;
    private EStatCategory statCategory;

    /// <summary>
    /// Initializes a new instance of <see cref="SaveRollData"/> with the specified difficulty class.
    /// </summary>
    /// <param name="difficultyClass">The difficulty class to beat.</param>
    public SaveRollData(int difficultyClass) {
        this.difficultyClass = difficultyClass;
    }

    /// <summary>
    /// Resolves a saving throw roll using the action context.
    /// </summary>
    /// <param name="ctx">The action context for the roll.</param>
    /// <returns>A <see cref="RollRecord"/> containing the result of the roll.</returns>
    public override RollRecord ResolveRoll(ActionContext ctx) {
        int roll = Random.Range(1, 21); // Simulate a d20 roll
        int bonus = 0;
        if (ctx.Target.Entity is CharacterCore sourceCharacter) {
            var stat = sourceCharacter.Stats.GetStat(statCategory);
            if (stat is IntComparableStat intStat) bonus = intStat.Value;
        }
        return new RollRecord(roll, difficultyClass, bonus);
    }
}

/// <summary>
/// Roll resolver for attack rolls, using the target's armor class and the source's attack bonus.
/// </summary>
public class AttackRollData : BaseRollResolver {
    /// <summary>
    /// Resolves an attack roll using the action context.
    /// </summary>
    /// <param name="ctx">The action context for the roll.</param>
    /// <returns>A <see cref="RollRecord"/> containing the result of the roll.</returns>
    public override RollRecord ResolveRoll(ActionContext ctx) {
        var ac = new IntComparableStat();
        if (ctx.Target.Entity is CharacterCore targetCharacter)
            ac = targetCharacter.Stats.GetStat(EStatCategory.AC) as IntComparableStat;
        int roll = Random.Range(1, 21); // Simulate a d20 roll
        int bonus = ctx.Source.Stats.GetStat(EStatCategory.AttackBonus) is IntComparableStat intStat ? intStat.Value : 0;
        return new RollRecord(roll, ac.Value, bonus);
    }
}
