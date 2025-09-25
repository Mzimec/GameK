using System.Collections.Generic;

/// <summary>
/// Defines the interface for retrieving available actions for a character.
/// </summary>
public interface IActions {
    /// <summary>
    /// Gets a list of actions that are currently available to the character.
    /// </summary>
    /// <returns>
    /// A list of <see cref="GameAction"/> instances representing the available actions.
    /// </returns>
    List<GameAction> GetAvailableActions();
}
