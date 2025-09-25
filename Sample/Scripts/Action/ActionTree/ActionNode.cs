using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a game action that can be executed and approximated.
/// </summary>
public interface IGameAction {
    /// <summary>
    /// Executes the action using the provided context.
    /// </summary>
    /// <param name="ctx">The action context.</param>
    void Execute(ActionContext ctx);

    /// <summary>
    /// Gets an approximation of the result of executing the action in the given context.
    /// </summary>
    /// <param name="ctx">The action context.</param>
    /// <returns>An <see cref="ActionApproximation"/> representing the expected result.</returns>
    ActionApproximation GetApproximatedResult(ActionContext ctx);
}

/// <summary>
/// Represents a node in an action tree, capable of executing child actions based on roll results and targeting logic.
/// </summary>
public class ActionNode : IGameAction {
    private Dictionary<ERollResult, List<IGameAction>> _children;
    private IRollResolver _rollResolver;
    private ITargetingSelector _targetingSelector;

    /// <summary>
    /// Selects targets for this action node using the targeting selector.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An enumerable of targetable entities.</returns>
    private IEnumerable<ITargetable> SelectTargets(ActionContext context) {
        return _targetingSelector.SelectTargets(context);
    }

    /// <summary>
    /// Sets the roll resolver for this node.
    /// </summary>
    /// <param name="resolver">The roll resolver to use.</param>
    public void SetRollResolver(IRollResolver resolver) => _rollResolver = resolver;

    /// <summary>
    /// Sets the targeting selector for this node.
    /// </summary>
    /// <param name="selector">The targeting selector to use.</param>
    public void SetTargetingSelector(ITargetingSelector selector) => _targetingSelector = selector;

    /// <summary>
    /// Sets the child actions for each roll result.
    /// </summary>
    /// <param name="children">A dictionary mapping roll results to lists of child actions.</param>
    public void SetChildren(Dictionary<ERollResult, List<IGameAction>> children) => _children = children;

    /// <summary>
    /// Executes the node's own logic. Override to implement custom behavior.
    /// </summary>
    /// <param name="context">The action context.</param>
    protected virtual void SelfExecute(ActionContext context) { }

    /// <summary>
    /// Approximates the result of the node's own logic. Override to implement custom behavior.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An <see cref="ActionApproximation"/> for the node's own logic.</returns>
    protected virtual ActionApproximation ApproximateSelfExecute(ActionContext context) => new ActionApproximation();

    /// <summary>
    /// Executes the action node, resolving rolls and executing child actions as appropriate.
    /// </summary>
    /// <param name="context">The action context.</param>
    public void Execute(ActionContext context) {
        bool isRollNeeded = _rollResolver != null;
        var targets = SelectTargets(context);
        foreach (var target in targets) {
            List<IGameAction> actions = new();
            var ctx = context.WithNewTarget(target);
            ERollResult rollResult = ERollResult.None;
            if (isRollNeeded) {
                var roll = _rollResolver.ResolveRoll(ctx);
                rollResult = roll.Result;
            }
            actions = _children.GetValueOrDefault(rollResult, new List<IGameAction>());
            SelfExecute(ctx);
            foreach (var action in actions) {
                action.Execute(ctx);
            }
        }
    }

    /// <summary>
    /// Gets an approximation of the result of executing this node and its children.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <returns>An <see cref="ActionApproximation"/> representing the expected result.</returns>
    public ActionApproximation GetApproximatedResult(ActionContext context) {
        var approximation = new ActionApproximation();
        bool isRollNeeded = _rollResolver != null;
        var targets = SelectTargets(context);
        foreach (var target in targets) {
            var ctx = context.WithNewTarget(target);
            approximation.Merge(ApproximateSelfExecute(ctx));
            Dictionary<ERollResult, float> successChance = new();
            if (isRollNeeded) {
                var roll = _rollResolver.ResolveRoll(ctx);
                successChance = _rollResolver.SuccessChance(roll);
            }
            foreach (var (key, acs) in _children) {
                float chance = successChance.GetValueOrDefault(key, 1f);
                foreach (var ac in acs) {
                    var childAprox = ac.GetApproximatedResult(ctx);
                    approximation.Merge(childAprox, chance);
                }
            }
        }
        return approximation;
    }
}

/// <summary>
/// Builder for constructing <see cref="ActionNode"/> instances with custom configuration.
/// </summary>
public class ActionNodeBuilder {
    private IRollResolver _rollResolver;
    private ITargetingSelector _targetingSelector;
    private readonly Dictionary<ERollResult, List<IGameAction>> _children = new();

    /// <summary>
    /// Gets the list of actions for a specific roll result, creating it if necessary.
    /// </summary>
    /// <param name="result">The roll result.</param>
    /// <returns>The list of actions for the specified result.</returns>
    private List<IGameAction> GetList(ERollResult result) {
        if (!_children.TryGetValue(result, out var list)) {
            list = new List<IGameAction>();
            _children[result] = list;
        }
        return list;
    }

    /// <summary>
    /// Sets the targeting selector for the node being built.
    /// </summary>
    /// <param name="targeting">The targeting selector to use.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder WithTargeting(ITargetingSelector targeting) {
        _targetingSelector = targeting;
        return this;
    }

    /// <summary>
    /// Sets the roll resolver for the node being built.
    /// </summary>
    /// <param name="rollResolver">The roll resolver to use.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder WithRoll(IRollResolver rollResolver) {
        _rollResolver = rollResolver;
        return this;
    }

    /// <summary>
    /// Adds an action to be executed for a specific roll result.
    /// </summary>
    /// <param name="result">The roll result.</param>
    /// <param name="action">The action to add.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder OnResult(ERollResult result, IGameAction action) {
        GetList(result).Add(action);
        return this;
    }

    /// <summary>
    /// Adds an action to be executed on a success roll result.
    /// </summary>
    /// <param name="action">The action to add.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder OnSuccess(IGameAction action) =>
        OnResult(ERollResult.Success, action);

    /// <summary>
    /// Adds an action to be executed on a failure roll result.
    /// </summary>
    /// <param name="action">The action to add.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder OnFailure(IGameAction action) =>
        OnResult(ERollResult.Failure, action);

    /// <summary>
    /// Adds an action to be executed on a critical success roll result.
    /// </summary>
    /// <param name="action">The action to add.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder OnCriticalSuccess(IGameAction action) =>
        OnResult(ERollResult.CriticalSuccess, action);

    /// <summary>
    /// Adds an action to be executed on a critical failure roll result.
    /// </summary>
    /// <param name="action">The action to add.</param>
    /// <returns>The builder instance.</returns>
    public ActionNodeBuilder OnCriticalFailure(IGameAction action) =>
        OnResult(ERollResult.CriticalFailure, action);

    /// <summary>
    /// Builds and returns the configured <see cref="ActionNode"/>.
    /// </summary>
    /// <returns>The constructed <see cref="ActionNode"/>.</returns>
    public ActionNode Build() {
        var node = new ActionNode();
        node.SetRollResolver(_rollResolver);
        node.SetTargetingSelector(_targetingSelector);
        node.SetChildren(_children);
        return node;
    }
}
