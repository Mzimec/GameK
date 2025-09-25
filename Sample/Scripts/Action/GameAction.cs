using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a composite game action that can be executed and approximated, containing child actions and metadata.
/// </summary>
public class GameAction : IGameAction {
    private string _name;
    /// <summary>
    /// Gets the name of the action.
    /// </summary>
    public string Name => _name;

    private string _description;
    /// <summary>
    /// Gets the description of the action.
    /// </summary>
    public string Description => _description;

    private List<IGameAction> _children;

    private int _baseCost;
    /// <summary>
    /// Gets the base cost of the action.
    /// </summary>
    public int BaseCost => _baseCost;

    /// <summary>
    /// Determines whether the action can be executed in the given context.
    /// </summary>
    /// <param name="context">The action context to evaluate.</param>
    /// <returns>True if the action can be executed; otherwise, false.</returns>
    private bool CanExecute(ActionContext context) {
        // Implement logic to determine if the action can be executed in the given context
        return true;
    }

    /// <summary>
    /// Executes the action and all child actions if allowed by the context.
    /// </summary>
    /// <param name="context">The action context for execution.</param>
    public void Execute(ActionContext context) {
        if (!CanExecute(context)) return;
        foreach (var action in _children) {
            action.Execute(context);
        }
    }

    /// <summary>
    /// Gets an approximation of the result of executing this action and its children in the given context.
    /// </summary>
    /// <param name="context">The action context for approximation.</param>
    /// <returns>An <see cref="ActionApproximation"/> representing the expected result.</returns>
    public ActionApproximation GetApproximatedResult(ActionContext context) {
        var approximation = new ActionApproximation();
        if (!CanExecute(context)) return approximation;
        foreach (var action in _children) {
            var atomicApprox = action.GetApproximatedResult(context);
            approximation.Merge(atomicApprox);
        }
        return approximation;
    }
}
