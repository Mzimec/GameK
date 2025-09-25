using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for character AI logic, including turn and action handling.
/// </summary>
public interface ICharacterAI {
    /// <summary>
    /// Executes the logic for the character's turn.
    /// </summary>
    /// <param name="state">The current game state.</param>
    void TakeTurn(GameState state);

    /// <summary>
    /// Executes a single action for the character and determines if the turn should end.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="isEndTunrWanted">Set to true if the turn should end after this action.</param>
    void TakeAction(GameState state, out bool isEndTunrWanted);
}

/// <summary>
/// Represents the current state of the game.
/// </summary>
public class GameState {
    // Represents the current state of the game
}

/// <summary>
/// Interface for comparing and scoring actions for a character.
/// </summary>
public interface IActionComparer {
    /// <summary>
    /// Scores the result of an action for the specified character.
    /// </summary>
    /// <param name="result">The approximation of the action's effects.</param>
    /// <param name="character">The character for whom the score is calculated.</param>
    /// <returns>A float value representing the desirability of the action.</returns>
    float Score(ActionApproximation result, CharacterCore character);
}

/// <summary>
/// Interface for grid-based operations, such as determining potential tiles for actions.
/// </summary>
public interface IGridOperations {
    /// <summary>
    /// Gets potential tiles for a given action and origin position.
    /// </summary>
    /// <param name="action">The game action to evaluate.</param>
    /// <param name="origin">The origin position.</param>
    /// <returns>An enumerable of potential tile positions.</returns>
    IEnumerable<Vector3Int> GetPotentialTiles(GameAction action, Vector3Int origin);
}

/// <summary>
/// Interface for operations related to action targeting.
/// </summary>
public interface IActionOperations {
    /// <summary>
    /// Gets potential targets for a given action and source character.
    /// </summary>
    /// <param name="action">The game action to evaluate.</param>
    /// <param name="source">The source character.</param>
    /// <returns>An enumerable of potential targets.</returns>
    IEnumerable<ITargetable> GetPotentialTargets(GameAction action, CharacterCore source);
}

/// <summary>
/// Abstract base class for character AI, providing core turn and action logic.
/// </summary>
public abstract class BaseCahracterAI : ICharacterAI {
    /// <inheritdoc/>
    public abstract void TakeTurn(GameState state);

    /// <inheritdoc/>
    public abstract void TakeAction(GameState state, out bool isEndTurnWanted);

    /// <summary>
    /// Evaluates a specific action and target, returning a score and the evaluated action/target.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="action">The action to evaluate.</param>
    /// <param name="target">The target to evaluate.</param>
    /// <returns>A tuple containing the score, evaluated action, and target.</returns>
    protected abstract (float score, GameAction evaluatedAction, ITargetable target) EvaluateAction(GameState state, GameAction action, ITargetable target);

    /// <summary>
    /// Selects the best action and target from a list of evaluated actions.
    /// </summary>
    /// <param name="evaluatedActions">A list of evaluated actions with scores.</param>
    /// <param name="isEndTurnWanted">Set to true if the turn should end after selection.</param>
    /// <returns>A tuple containing the selected action and target.</returns>
    protected abstract (GameAction action, ITargetable target) SelectBestAction(List<(float score, GameAction evaluatedAction, ITargetable target)> evaluatedActions,
        out bool isEndTurnWanted);

    /// <summary>
    /// Gets potential targets for a given game state and action.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="action">The action to evaluate.</param>
    /// <returns>An enumerable of potential targets.</returns>
    protected abstract IEnumerable<ITargetable> GetPotentialTargets(GameState state, GameAction action);
}

/// <summary>
/// Default implementation of character AI, handling turn and action selection.
/// </summary>
public class CharacterAI : BaseCahracterAI {
    private CharacterCore _owner;
    private IActionComparer _actionComparer;
    private IActionOperations _actionOperation;

    /// <summary>
    /// Initializes a new instance of <see cref="CharacterAI"/> for the specified character.
    /// </summary>
    /// <param name="owner">The character core that owns this AI.</param>
    public CharacterAI(CharacterCore owner) {
        _owner = owner;
    }

    /// <inheritdoc/>
    public override void TakeTurn(GameState state) {
        bool isEndTurnWanted = false;
        while (!isEndTurnWanted) TakeAction(state, out isEndTurnWanted);
    }

    /// <inheritdoc/>
    public override void TakeAction(GameState state, out bool isEndTurnWanted) {
        var evaluatedActions = new List<(float, GameAction, ITargetable)>();
        List<GameAction> availableActions = _owner.Actions.GetAvailableActions();
        foreach (var action in availableActions) {
            var potentialTargets = GetPotentialTargets(state, null);
            foreach (var target in potentialTargets) {
                var evaluation = EvaluateAction(state, action, target);
                evaluatedActions.Add(evaluation);
            }
        }
        var bestAction = SelectBestAction(evaluatedActions, out isEndTurnWanted);
        if (bestAction.action == null || bestAction.target == null) return;
        bestAction.action.Execute(new ActionContext(_owner, bestAction.target, new TriggerContext(), bestAction.action));
    }

    /// <inheritdoc/>
    protected override (float score, GameAction evaluatedAction, ITargetable target) EvaluateAction(GameState state, GameAction action, ITargetable target) {
        var actionResult = action.GetApproximatedResult(new ActionContext(_owner, target, new TriggerContext(), action));
        var score = _actionComparer.Score(actionResult, _owner);
        return (score, action, target);
    }

    /// <inheritdoc/>
    protected override (GameAction action, ITargetable target) SelectBestAction(List<(float score, GameAction evaluatedAction, ITargetable target)> evaluatedActions,
        out bool isEndTurnWanted) {
        isEndTurnWanted = true;
        if (evaluatedActions.Count == 0) return (null, null);

        evaluatedActions.Sort((a, b) => b.score.CompareTo(a.score)); // Descending order

        float baseScore = 0;

        if (evaluatedActions[0].score <= baseScore) return (null, null); // No action is better than current state

        isEndTurnWanted = false;
        return (evaluatedActions[0].evaluatedAction, evaluatedActions[0].target); // Placeholder for best action selection
    }

    /// <inheritdoc/>
    protected override IEnumerable<ITargetable> GetPotentialTargets(GameState state, GameAction action) {
        return _actionOperation.GetPotentialTargets(action, _owner);
    }
}
