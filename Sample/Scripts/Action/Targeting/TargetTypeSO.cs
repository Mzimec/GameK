using UnityEngine;

/// <summary>
/// ScriptableObject that defines logic for validating whether a target is valid for a given source.
/// </summary>
public abstract class TargetTypeSO : ScriptableObject {
    /// <summary>
    /// Determines if the specified target is valid for the given source character.
    /// </summary>
    /// <param name="source">The source character attempting to target.</param>
    /// <param name="target">The target to validate.</param>
    /// <returns>True if the target is valid; otherwise, false.</returns>
    public abstract bool IsValidTarget(CharacterCore source, ITargetable target);
}
