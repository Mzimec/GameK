using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Interface for selecting targets for an action.
/// </summary>
public interface ITargetingSelector {
    /// <summary>
    /// Selects targets based on the provided action context.
    /// </summary>
    /// <param name="ctx">The action context.</param>
    /// <returns>An enumerable of targetable entities.</returns>
    IEnumerable<ITargetable> SelectTargets(ActionContext ctx);
}

/// <summary>
/// Base class for implementing target selection logic.
/// </summary>
public abstract class TargetingSelector : ITargetingSelector {
    private TargetTypeSO targetType;

    /// <summary>
    /// Selects targets for the action by filtering entities on selected tiles.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An enumerable of valid targetable entities.</returns>
    public virtual IEnumerable<ITargetable> SelectTargets(ActionContext context) {
        var tiles = SelectTiles(context);
        var entities = GridOperationManager.Instance.GetEntities(tiles);
        var targetables = entities.OfType<ITargetable>();
        return targetables.Where(t => IsValidTarget(context, t));
    }

    /// <summary>
    /// Selects tiles relevant to the action. Override to implement custom tile selection logic.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An enumerable of tile positions.</returns>
    protected virtual IEnumerable<Vector3Int> SelectTiles(ActionContext context) => new List<Vector3Int>();

    /// <summary>
    /// Determines if a target is valid for the action.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <param name="target">The target to validate.</param>
    /// <returns>True if the target is valid; otherwise, false.</returns>
    private bool IsValidTarget(ActionContext context, ITargetable target) {
        return targetType.IsValidTarget(context.Source, target);
    }
}

/// <summary>
/// Targeting selector that selects the source itself as the target.
/// </summary>
public class SelfTargetingSelector : TargetingSelector {
    /// <summary>
    /// Selects the source character as the only target.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An enumerable containing the source as the target, or logs an error if not targetable.</returns>
    public override IEnumerable<ITargetable> SelectTargets(ActionContext context) {
        var results = new List<ITargetable>();
        if (context.Source == null)
            Debug.LogError($"Source is null in {context.Root.Name} action.");
        if (context.Source is ITargetable targetable)
            yield return targetable;
        else
            Debug.LogError($"Source is not targetable for self targeting action: {context.Root.Name}.");
    }
}
