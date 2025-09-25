using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an entity that can be targeted by actions.
/// </summary>
public interface ITargetable {
    /// <summary>
    /// Gets the position of the target in the world/grid.
    /// </summary>
    Vector3Int Position { get; }

    /// <summary>
    /// Gets the associated GameObject for the target.
    /// </summary>
    GameObject GameObject { get; }

    /// <summary>
    /// Gets the entity data for the target.
    /// </summary>
    IEntity Entity { get; }
}

/// <summary>
/// Provides contextual information for executing or evaluating a game action, including source, target, trigger, and root action.
/// </summary>
public class ActionContext {
    /// <summary>
    /// The character that is the source of the action.
    /// </summary>
    public CharacterCore Source { get; }

    /// <summary>
    /// The target of the action.
    /// </summary>
    public ITargetable Target { get; }

    /// <summary>
    /// The trigger context associated with the action.
    /// </summary>
    public TriggerContext TriggerContext { get; }

    /// <summary>
    /// The root game action for this context.
    /// </summary>
    public GameAction Root { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ActionContext"/> with the specified source, target, trigger context, and root action.
    /// </summary>
    /// <param name="source">The source character.</param>
    /// <param name="target">The target entity.</param>
    /// <param name="triggerContext">The trigger context.</param>
    /// <param name="root">The root game action.</param>
    public ActionContext(CharacterCore source, ITargetable target, TriggerContext triggerContext, GameAction root) {
        this.Source = source;
        this.Target = target;
        this.TriggerContext = triggerContext;
        this.Root = root;
    }

    /// <summary>
    /// Returns a new <see cref="ActionContext"/> with a different target.
    /// </summary>
    /// <param name="newTarget">The new target entity.</param>
    /// <returns>A new <see cref="ActionContext"/> instance with the updated target.</returns>
    public ActionContext WithNewTarget(ITargetable newTarget) {
        return new ActionContext(this.Source, newTarget, this.TriggerContext, this.Root);
    }

    /// <summary>
    /// Returns a new <see cref="ActionContext"/> with a different source.
    /// </summary>
    /// <param name="newSource">The new source character.</param>
    /// <returns>A new <see cref="ActionContext"/> instance with the updated source.</returns>
    public ActionContext WithNewSource(CharacterCore newSource) {
        return new ActionContext(newSource, this.Target, this.TriggerContext, this.Root);
    }

    /// <summary>
    /// Returns a new <see cref="ActionContext"/> with a different trigger context.
    /// </summary>
    /// <param name="newTriggerContext">The new trigger context.</param>
    /// <returns>A new <see cref="ActionContext"/> instance with the updated trigger context.</returns>
    public ActionContext WithNewTriggerContext(TriggerContext newTriggerContext) {
        return new ActionContext(this.Source, this.Target, newTriggerContext, this.Root);
    }
}

/// <summary>
/// Maintains activation state for triggers and targets within an action context.
/// </summary>
public class TriggerContext {
    private readonly HashSet<(string TriggerId, string TargetId)> _activated
        = new HashSet<(string, string)>();

    /// <summary>
    /// Determines if the specified trigger can be activated for the given target.
    /// Returns true if this is the first activation for the pair.
    /// </summary>
    /// <param name="triggerId">The unique identifier for the trigger.</param>
    /// <param name="targetId">The unique identifier for the target.</param>
    /// <returns>True if the trigger can be activated; otherwise, false.</returns>
    public bool CanActivate(string triggerId, string targetId) {
        return _activated.Add((triggerId, targetId));
    }
}
