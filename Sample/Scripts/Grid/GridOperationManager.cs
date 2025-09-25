using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Singleton managing grid operations. Provides utility methods to get 
/// tile positions in various shapes and to retrieve entities on those tiles.
/// </summary>
public class GridOperationManager : ScriptableSingleton<GridOperationManager> {
    private Grid grid;

    /// <summary>
    /// Ensures the grid reference is initialized.
    /// </summary>
    private void EnsureGrid() {
        if (grid == null) {
            grid = FindFirstObjectByType<Grid>();
            if (grid == null) {
                Debug.LogError("No Grid found in the scene.");
            }
        }
    }

    /// <summary>
    /// Returns all tile positions within a circular radius around the center.
    /// Uses Euclidean distance.
    /// </summary>
    /// <param name="center">The center tile position.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>An enumerable of tile positions within the circle.</returns>
    public IEnumerable<Vector3Int> GetTilesInRadius(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                Vector3Int offset = new Vector3Int(x, y, 0);
                if (offset.magnitude <= radius) {
                    yield return center + offset;
                }
            }
        }
    }

    /// <summary>
    /// Returns all tile positions in a square area around the center.
    /// </summary>
    /// <param name="center">The center tile position.</param>
    /// <param name="radius">The radius of the square in X and Y directions.</param>
    /// <returns>An enumerable of tile positions within the square.</returns>
    public IEnumerable<Vector3Int> GetTilesInSquare(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                yield return center + new Vector3Int(x, y, 0);
            }
        }
    }

    /// <summary>
    /// Returns all tile positions in a diamond shape (Manhattan distance) around the center.
    /// </summary>
    /// <param name="center">The center tile position.</param>
    /// <param name="radius">The maximum Manhattan distance.</param>
    /// <returns>An enumerable of tile positions within the diamond.</returns>
    public IEnumerable<Vector3Int> GetTilesInDiamond(Vector3Int center, int radius) {
        EnsureGrid();
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= radius) {
                    yield return center + new Vector3Int(x, y, 0);
                }
            }
        }
    }

    /// <summary>
    /// Returns all tile positions in a straight line from a start position.
    /// </summary>
    /// <param name="start">The starting tile position.</param>
    /// <param name="direction">The direction vector (unit vector).</param>
    /// <param name="length">The length of the line.</param>
    /// <returns>An enumerable of tile positions along the line.</returns>
    public IEnumerable<Vector3Int> GetTilesInLine(Vector3Int start, Vector3Int direction, int length) {
        EnsureGrid();
        for (int i = 1; i <= length; i++) {
            yield return start + direction * i;
        }
    }

    /// <summary>
    /// Returns the entity at a given tile position, if any.
    /// </summary>
    /// <param name="tilePosition">The tile position to check.</param>
    /// <returns>The entity at the tile, or null if none exists.</returns>
    public IEntity GetEntityAtTile(Vector3Int tilePosition) {
        EnsureGrid();
        RaycastHit2D hit = Physics2D.Raycast(grid.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0), Vector2.zero);
        if (hit.collider != null) {
            return hit.collider.GetComponent<IEntity>();
        }
        return null;
    }

    /// <summary>
    /// Returns all entities present on the given tiles.
    /// </summary>
    /// <param name="tiles">An enumerable of tile positions.</param>
    /// <returns>A list of entities found on these tiles.</returns>
    public IEnumerable<IEntity> GetEntities(IEnumerable<Vector3Int> tiles) {
        var results = new List<IEntity>();
        foreach(var tile in tiles) {
            var entity = GetEntityAtTile(tile);
            if (entity != null) {
                results.Add(entity);
            }
        }
        return results;
    }
}