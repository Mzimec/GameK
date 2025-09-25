using System.Collections.Generic;


/// <summary>
/// An interface for providing stats in the game.
/// </summary>
public interface IStatProvider {
    /// <summary>
    /// Gets all stats provided by this stat provider.
    /// </summary>
    /// <returns>Reurns all stats by this interface</returns>
    public Dictionary<StatTypeSO, IStatBase> GetStats();
}